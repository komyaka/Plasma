using System.Collections.Generic;

namespace Plazma.Models
{
    public class UserRolePresetItem
    {
        public string Key { get; set; }
        public int Value { get; set; }
    }

    public class UserAdminChapterViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BinCode { get; set; }
    }

    public class UserAdminUserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
        public int Funct { get; set; }
        public string RolesDisplay { get; set; }
    }

    public class UserAdminFormViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
        public int[] SelectedChapters { get; set; }
        public List<UserAdminChapterViewModel> Chapters { get; set; }
        public List<UserRolePresetItem> Presets { get; set; }
        public int CalculatedFunct { get; set; }
    }
}
