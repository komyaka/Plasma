using System.Collections.Generic;

namespace Plazma.Models
{
    public static class UserRolePresets
    {
        public const int UserAdmin = 2048;
        public const int Admin = 34815;
        public const int Master = 651;
        public const int Operator = 299;
        public const int Storekeeper = 31;
        public const int Shipper = 1675;
        public const int Finance = 193;
        public const int Viewer = 651; // Алиас Мастер для сценария просмотра в Workgroup.

        public static List<UserRolePresetItem> GetPresetItems()
        {
            return new List<UserRolePresetItem>
            {
                new UserRolePresetItem { Key = "Администратор", Value = Admin },
                new UserRolePresetItem { Key = "Мастер цеха", Value = Master },
                new UserRolePresetItem { Key = "Оператор CNC", Value = Operator },
                new UserRolePresetItem { Key = "Кладовщик", Value = Storekeeper },
                new UserRolePresetItem { Key = "Отгрузчик", Value = Shipper },
                new UserRolePresetItem { Key = "Финансист", Value = Finance },
                new UserRolePresetItem { Key = "Просмотрщик", Value = Viewer }
            };
        }
    }
}
