using Plazma.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Plazma.Controllers
{
    public class UsersAdminController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            ActionResult accessResult = EnsureUserAdminAccess(nameof(Index));
            if (accessResult != null)
            {
                return accessResult;
            }

            try
            {
                List<UserAdminChapterViewModel> chapters = LoadChapters();
                List<UserAdminUserViewModel> usersList = LoadUsers(chapters);
                return View(usersList);
            }
            catch (Exception ex)
            {
                DatabaseLogger.Log("UsersAdminController.Index", ex.Message, ex.ToString(), User?.Identity?.Name);
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            ActionResult accessResult = EnsureUserAdminAccess(nameof(Create));
            if (accessResult != null)
            {
                return accessResult;
            }

            try
            {
                return View(BuildFormModel(string.Empty, string.Empty, new int[0], null));
            }
            catch (Exception ex)
            {
                DatabaseLogger.Log("UsersAdminController.CreateGet", ex.Message, ex.ToString(), User?.Identity?.Name);
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string domain, string name, int[] chapters)
        {
            ActionResult accessResult = EnsureUserAdminAccess(nameof(Create));
            if (accessResult != null)
            {
                return accessResult;
            }

            try
            {
                string safeDomain = (domain ?? string.Empty).Trim();
                string safeName = (name ?? string.Empty).Trim();
                List<UserAdminChapterViewModel> allChapters = LoadChapters();
                int funct = CalculateFunct(chapters, allChapters);

                if (string.IsNullOrWhiteSpace(safeName))
                {
                    ModelState.AddModelError("", "Поле NAME обязательно.");
                }

                if (safeDomain.Length > 50 || safeName.Length > 50)
                {
                    ModelState.AddModelError("", "NAME и DOMAIN не должны превышать 50 символов.");
                }

                if (UserExists(safeDomain, safeName))
                {
                    ModelState.AddModelError("", "Пользователь с таким DOMAIN\\NAME уже существует.");
                }

                if (!ModelState.IsValid)
                {
                    return View(BuildFormModel(safeDomain, safeName, chapters, null, allChapters, funct));
                }

                using (SqlConnection connection = new SqlConnection(Constants.bdconnectionstring))
                using (SqlCommand command = new SqlCommand("INSERT INTO USERS (DOMAIN, NAME, FUNCT) VALUES (@Domain, @Name, @Funct)", connection))
                {
                    command.Parameters.AddWithValue("@Domain", safeDomain);
                    command.Parameters.AddWithValue("@Name", safeName);
                    command.Parameters.AddWithValue("@Funct", funct);
                    connection.Open();
                    command.ExecuteNonQuery();
                }

                DatabaseLogger.Log(
                    "UsersAdminController.Create",
                    "Создан пользователь " + safeDomain + "\\" + safeName + " с FUNCT=" + funct,
                    string.Empty,
                    User?.Identity?.Name);

                TempData["UsersAdminMessage"] = "Пользователь успешно создан.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                DatabaseLogger.Log("UsersAdminController.Create", ex.Message, ex.ToString(), User?.Identity?.Name);
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ActionResult accessResult = EnsureUserAdminAccess(nameof(Edit));
            if (accessResult != null)
            {
                return accessResult;
            }

            if (id <= 0)
            {
                return new HttpStatusCodeResult(400);
            }

            try
            {
                UserAdminUserViewModel userModel = GetUserById(id);
                if (userModel == null)
                {
                    return HttpNotFound();
                }

                int[] selectedChapters = LoadChapters()
                    .Where(x => (userModel.Funct & x.BinCode) == x.BinCode)
                    .Select(x => x.BinCode)
                    .ToArray();

                return View(BuildFormModel(userModel.Domain, userModel.Name, selectedChapters, id));
            }
            catch (Exception ex)
            {
                DatabaseLogger.Log("UsersAdminController.EditGet", ex.Message, ex.ToString(), User?.Identity?.Name);
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, string domain, string name, int[] chapters)
        {
            ActionResult accessResult = EnsureUserAdminAccess(nameof(Edit));
            if (accessResult != null)
            {
                return accessResult;
            }

            if (id <= 0)
            {
                return new HttpStatusCodeResult(400);
            }

            try
            {
                UserAdminUserViewModel existingUser = GetUserById(id);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }

                string safeDomain = (domain ?? string.Empty).Trim();
                string safeName = (name ?? string.Empty).Trim();
                List<UserAdminChapterViewModel> allChapters = LoadChapters();
                int funct = CalculateFunct(chapters, allChapters);
                bool isSelf = IsCurrentUser(existingUser.Domain, existingUser.Name);

                if (string.IsNullOrWhiteSpace(safeName))
                {
                    ModelState.AddModelError("", "Поле NAME обязательно.");
                }

                if (safeDomain.Length > 50 || safeName.Length > 50)
                {
                    ModelState.AddModelError("", "NAME и DOMAIN не должны превышать 50 символов.");
                }

                if (isSelf && (funct & UserRolePresets.UserAdmin) == 0)
                {
                    ModelState.AddModelError("", "Нельзя снять право UserAdmin у текущего пользователя.");
                    DatabaseLogger.Log(
                        "UsersAdminController.Edit",
                        "Попытка self-lockout: " + existingUser.Domain + "\\" + existingUser.Name,
                        string.Empty,
                        User?.Identity?.Name);
                }

                if (!string.Equals(existingUser.Domain ?? string.Empty, safeDomain, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(existingUser.Name ?? string.Empty, safeName, StringComparison.OrdinalIgnoreCase))
                {
                    if (UserExists(safeDomain, safeName))
                    {
                        ModelState.AddModelError("", "Пользователь с таким DOMAIN\\NAME уже существует.");
                    }
                }

                if (!ModelState.IsValid)
                {
                    return View(BuildFormModel(safeDomain, safeName, chapters, id, allChapters, funct));
                }

                using (SqlConnection connection = new SqlConnection(Constants.bdconnectionstring))
                using (SqlCommand command = new SqlCommand("UPDATE USERS SET DOMAIN=@Domain, NAME=@Name, FUNCT=@Funct WHERE ID=@ID", connection))
                {
                    command.Parameters.AddWithValue("@Domain", safeDomain);
                    command.Parameters.AddWithValue("@Name", safeName);
                    command.Parameters.AddWithValue("@Funct", funct);
                    command.Parameters.AddWithValue("@ID", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }

                DatabaseLogger.Log(
                    "UsersAdminController.Edit",
                    "Изменён пользователь ID=" + id + " (" + safeDomain + "\\" + safeName + "), FUNCT=" + funct,
                    string.Empty,
                    User?.Identity?.Name);

                TempData["UsersAdminMessage"] = "Пользователь успешно изменён.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                DatabaseLogger.Log("UsersAdminController.Edit", ex.Message, ex.ToString(), User?.Identity?.Name);
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            ActionResult accessResult = EnsureUserAdminAccess(nameof(Delete));
            if (accessResult != null)
            {
                return accessResult;
            }

            if (id <= 0)
            {
                return new HttpStatusCodeResult(400);
            }

            try
            {
                UserAdminUserViewModel existingUser = GetUserById(id);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }

                if (IsCurrentUser(existingUser.Domain, existingUser.Name))
                {
                    DatabaseLogger.Log(
                        "UsersAdminController.Delete",
                        "Попытка удалить самого себя: " + existingUser.Domain + "\\" + existingUser.Name,
                        string.Empty,
                        User?.Identity?.Name);

                    TempData["UsersAdminError"] = "Нельзя удалить текущего пользователя.";
                    return RedirectToAction("Index");
                }

                using (SqlConnection connection = new SqlConnection(Constants.bdconnectionstring))
                using (SqlCommand command = new SqlCommand("DELETE FROM USERS WHERE ID=@ID", connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }

                DatabaseLogger.Log(
                    "UsersAdminController.Delete",
                    "Удалён пользователь ID=" + id + " (" + existingUser.Domain + "\\" + existingUser.Name + ")",
                    string.Empty,
                    User?.Identity?.Name);

                TempData["UsersAdminMessage"] = "Пользователь удалён.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                DatabaseLogger.Log("UsersAdminController.Delete", ex.Message, ex.ToString(), User?.Identity?.Name);
                return new HttpStatusCodeResult(500);
            }
        }

        private ActionResult EnsureUserAdminAccess(string actionName)
        {
            currentuser = users.getCurrentUser();
            ViewBag.User = currentuser;
            if ((currentuser.Privilegies & UserRolePresets.UserAdmin) != 0)
            {
                return null;
            }

            DatabaseLogger.Log(
                "UsersAdminController." + actionName,
                "Запрещён доступ без бита UserAdmin.",
                string.Empty,
                User?.Identity?.Name);

            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        private bool IsCurrentUser(string domain, string name)
        {
            return string.Equals((currentuser.domain ?? string.Empty).Trim(), (domain ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase) &&
                   string.Equals((currentuser.name ?? string.Empty).Trim(), (name ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private bool UserExists(string domain, string name)
        {
            using (SqlConnection connection = new SqlConnection(Constants.bdconnectionstring))
            using (SqlCommand command = new SqlCommand("SELECT COUNT(1) FROM USERS WHERE NAME=@Name AND ISNULL(DOMAIN, N'')=@Domain", connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Domain", domain ?? string.Empty);
                connection.Open();
                object scalar = command.ExecuteScalar();
                int count = scalar is int result ? result : 0;
                return count > 0;
            }
        }

        private UserAdminUserViewModel GetUserById(int id)
        {
            using (SqlConnection connection = new SqlConnection(Constants.bdconnectionstring))
            using (SqlCommand command = new SqlCommand("SELECT ID, NAME, DOMAIN, FUNCT FROM USERS WHERE ID=@ID", connection))
            {
                command.Parameters.AddWithValue("@ID", id);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new UserAdminUserViewModel
                    {
                        Id = Convert.ToInt32(reader["ID"]),
                        Name = Convert.ToString(reader["NAME"] ?? string.Empty),
                        Domain = Convert.ToString(reader["DOMAIN"] ?? string.Empty),
                        Funct = Convert.ToInt32(reader["FUNCT"] == DBNull.Value ? 0 : reader["FUNCT"])
                    };
                }
            }
        }

        private List<UserAdminUserViewModel> LoadUsers(List<UserAdminChapterViewModel> chapters)
        {
            List<UserAdminUserViewModel> result = new List<UserAdminUserViewModel>();
            using (SqlConnection connection = new SqlConnection(Constants.bdconnectionstring))
            using (SqlCommand command = new SqlCommand("SELECT ID, NAME, DOMAIN, FUNCT FROM USERS ORDER BY DOMAIN, NAME", connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int funct = Convert.ToInt32(reader["FUNCT"] == DBNull.Value ? 0 : reader["FUNCT"]);
                        List<string> roleNames = chapters
                            .Where(x => (funct & x.BinCode) == x.BinCode)
                            .Select(x => x.Name)
                            .ToList();

                        result.Add(new UserAdminUserViewModel
                        {
                            Id = Convert.ToInt32(reader["ID"]),
                            Name = Convert.ToString(reader["NAME"] ?? string.Empty),
                            Domain = Convert.ToString(reader["DOMAIN"] ?? string.Empty),
                            Funct = funct,
                            RolesDisplay = roleNames.Count == 0 ? "—" : string.Join(", ", roleNames)
                        });
                    }
                }
            }

            return result;
        }

        private List<UserAdminChapterViewModel> LoadChapters()
        {
            List<UserAdminChapterViewModel> chapters = new List<UserAdminChapterViewModel>();
            using (SqlConnection connection = new SqlConnection(Constants.bdconnectionstring))
            using (SqlCommand command = new SqlCommand("SELECT ID, CHAPTER, BINCODE FROM CHAPTERS ORDER BY BINCODE", connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        chapters.Add(new UserAdminChapterViewModel
                        {
                            Id = Convert.ToInt32(reader["ID"]),
                            Name = Convert.ToString(reader["CHAPTER"] ?? string.Empty).Trim(),
                            BinCode = Convert.ToInt32(reader["BINCODE"])
                        });
                    }
                }
            }

            return chapters;
        }

        private int CalculateFunct(int[] selectedCodes, List<UserAdminChapterViewModel> chapters)
        {
            if (selectedCodes == null || selectedCodes.Length == 0 || chapters == null || chapters.Count == 0)
            {
                return 0;
            }

            HashSet<int> selected = new HashSet<int>(selectedCodes);
            int funct = 0;
            foreach (UserAdminChapterViewModel chapter in chapters)
            {
                if (selected.Contains(chapter.BinCode))
                {
                    funct |= chapter.BinCode;
                }
            }

            return funct;
        }

        private UserAdminFormViewModel BuildFormModel(string domain, string name, int[] selectedChapters, int? id, List<UserAdminChapterViewModel> chapters = null, int? calculatedFunct = null)
        {
            List<UserAdminChapterViewModel> safeChapters = chapters ?? LoadChapters();
            int[] safeSelected = selectedChapters ?? new int[0];
            int funct = calculatedFunct ?? CalculateFunct(safeSelected, safeChapters);

            return new UserAdminFormViewModel
            {
                Id = id,
                Domain = domain ?? string.Empty,
                Name = name ?? string.Empty,
                SelectedChapters = safeSelected,
                Chapters = safeChapters,
                Presets = UserRolePresets.GetPresetItems(),
                CalculatedFunct = funct
            };
        }
    }
}
