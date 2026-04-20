using Plazma.Models.ClassAPI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Plazma.Controllers.Users;

namespace Plazma.Controllers
{
    public class adrController : Controller
    {
        // GET: adr
        public _user currentuser;
        public Users users = new Users();

        protected struct cncNames
        {
            public string newFileName;
            public string oldFileName;
            public cncNames(string newName, string oldName = "")
            {
                newFileName = newName;
                oldFileName = (oldName == "") ? newName : oldName;
            }
        }
        protected admin ADM = new admin { };
        public static PartsClass parts = new PartsClass(0);
        public List<string> filelist = new List<string> { }; // Хранит имена файлов при загрузке на сервер.
        public ActionResult newCNC2(IEnumerable<HttpPostedFileBase> uploads)
        {
            int result = 0;
            List<cncNames> files = new List<cncNames> { };
            files.Clear();
            string tmpfile = "";
            string path = @"D:/PlazmaProgs/NewFiles/";
            foreach (var file in uploads)
            {
                if (file != null)
                {
                    string fileName = System.IO.Path.GetFileName(file.FileName);
                    tmpfile = ADM.getNewFileName(fileName, path);

                    file.SaveAs(path + tmpfile);
                    files.Add(new cncNames(fileName, tmpfile));
                }
            }
            foreach (cncNames s in files)
            {
                cnc Fcnc = new cnc(s.newFileName);
                PartsClass._CNC nCNC = new PartsClass._CNC(
                    Filename: s.newFileName,
                    OriginalFile: s.oldFileName,
                    quantity: Fcnc.QuantityCut,
                    Tickness: cnc.gettiknessfromname(s.oldFileName),
                    Realtickness: cnc.gettiknessfromname(s.oldFileName),
                    material: PartsClass.GetMaterialFromName(s.oldFileName),
                    // ширина листа ???
                    width: (int)(Fcnc.Sheet.maxX - Fcnc.Sheet.minX),
                    heigth: (int)(Fcnc.Sheet.maxY - Fcnc.Sheet.minY)
                    );
                int id = parts.AddCNCtoBD(nCNC);
                foreach (cnc._part p in Fcnc.Parts)
                {
                    PartsClass._Part newPart = new PartsClass._Part(
                        name: p.Name,
                        quantity: p.quantity,
                        quantitysum: p.quantity * Fcnc.QuantityCut,
                        Quantitycutted: 0,
                        Tickness: cnc.gettiknessfromname(s.oldFileName),
                        SizeX: p.size.x.ToString(),
                        SizeY: p.size.y.ToString(),
                        CncId: id.ToString());
                    parts.AddPartToBD(newPart);
                }
            }
            if (result < 0) Console.WriteLine("ошибка файлы не добавлены");
            return Json(files[0], JsonRequestBehavior.AllowGet);
        }
        public ActionResult DelCNC(int ID)
        {
            int status = -ID;
            try
            {
                parts.DeleteCNC(ID);
                status = ID;
            }
            catch { }
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DoneCNC(int ID)
        {
            int status = -ID;
            List<string> ordersdone = new List<string> { };
            try
            {
                ordersdone = parts.changestatus(ID);
                status = ID;
            }
            catch(Exception e) { Console.WriteLine(e.Message); }
            return Json(new {status = ID,Lst=ordersdone }, JsonRequestBehavior.AllowGet) ;
        }
        public ActionResult NewSheet(int mat, string tikn, int WIDTH, int HEIGTH, int Quantity = 1, string OWNER = "МОНТАЖНИК", string DOC = "", string Date = "")
        {
           // int result = 0;
            currentuser = users.getCurrentUser();
            PartsClass partsClass = new PartsClass(0);
            if (((currentuser.Privilegies & partsClass.getChapterCode("addSheet")) == 0)) return Redirect(Url.RouteUrl(new { controller = "HOME", action = "Index" }));
            ViewBag.User = currentuser;
            PartsClass._sheet nSheet;
            nSheet.Matherial = mat;
            nSheet.Id = 0;
            nSheet.Name = "NULL";
            nSheet.Owner = OWNER;
            nSheet.Parts = "";
            nSheet.Quantity = 1;
            nSheet.Reserve1 = "";
            nSheet.Reserve2 = "";
            nSheet.Reserve3 = "";
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            nSheet.Tickness = (float)Convert.ToDouble(tikn.Replace(",", "."));
            nSheet.CNCFILE = "";
            nSheet.Heigth = HEIGTH;
            nSheet.Width = WIDTH;
            nSheet.Status = "";
            if (Date.Length < 5)
            {
                nSheet.Date = DateTime.Now;
            }
            else
                nSheet.Date = DateTime.Parse(Date.Replace(".", "/"), CultureInfo.CreateSpecificCulture("fr-FR"));

            nSheet.Document = DOC;
            for (int i = 1; i <= Quantity; i++) { partsClass.AddSheettoBD(nSheet); }
            return Json(nSheet);
        }
        public ActionResult DeleteSheet(int id=-1, float tickness=-1, int width=-1, int heigth=-1, int matherial=-1, string doc="*")
        {
            object result;
            try
            {
                if (id > 0) parts.FreeRequestToBD("Delete * froms heets Where id=" + id.ToString());
                else if ((tickness > 0) && (width > 0) && (heigth > 0) && (matherial > 0) && (doc != "*")) parts.FreeRequestToBD("delete from sheets where NAME is null and TICKNESS=" + tickness.ToString()+ " and WIDTH=" + width.ToString()+ "and HEIGTH=" + heigth.ToString()+ " and MATHERIAL=" + matherial+(doc=="*"?"":("and document="+doc)));
                else if (tickness > 0)
                { return Json("no tickness"); }
                else if (width > 0)
                { return Json("no width"); }
                else if (heigth > 0)
                { return Json("no heigth"); }
                else if (matherial > 0)
                { return Json("no matherial"); }
            }
            catch (Exception e) 
            {
                return Json(e.Message);
            }
            result = 0;
            return Json(result);
        }
        public ActionResult MarkSheet(int Sheetid,string name="",int cncID=-1)
        {
            if ((name.Length < 1) && (cncID < 0)) { return Json("No program ID"); }
            try
            {
                if (cncID > 0)
                    parts.FreeRequestToBD("update SHEETS set name=(select FILENAME  from CNCFILES where id=" + cncID.ToString() + ") where id=" + Sheetid.ToString());
                else parts.FreeRequestToBD("Update sheets set name=" + name);
            }
            catch (Exception e){ return Json(e.Message); }
            int result = 0;
            return Json(result);
        }
        public ActionResult PartListInCNC(int cncId)
        {
            int result = 0;
            //var response = from prt in parts.Parts where (prt.CNCID.Trim() == cncId.ToString());
            return Json(result);
        }
        public ActionResult Partsrecovery()
        {

            parts.readCNC("select * from CNCFILES order by id desc");
            string path1 = Constants.CNCPath;
            string path2 = Constants.CNCPath+@"\arhive";
            int result = 0;
            //var response = from prt in parts.Parts where (prt.CNCID.Trim() == cncId.ToString());
            foreach (PartsClass._CNC s in parts.CNCs)
            {
                if (System.IO.File.Exists(s.FileName))
                {
                    cnc Fcnc = new cnc(s.FileName);
                    foreach (cnc._part p in Fcnc.Parts)
                    {
                        parts.FreeRequestToBD("update parts set SIZE_X=" + p.size.x + ",SIZE_Y=" + p.size.y + " where NAME like '%" + p.Name + "%' and CNCID=" + s.Id.ToString());
                    }
                    result++;
                }
                else
                if (System.IO.File.Exists(s.FileName.Replace(path1,path2)))
                {
                    cnc Fcnc = new cnc(s.FileName);
                    foreach (cnc._part p in Fcnc.Parts)
                    {
                        parts.FreeRequestToBD("update parts set SIZE_X=" + p.size.x + ",SIZE_Y=" + p.size.y + " where NAME like '%" + p.Name + "%' and CNCID=" + s.Id.ToString());
                    }
                    result++;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}