using System.Collections.Generic;

namespace Plazma.Models.NC
{
    public struct MaterialInfo
    {
        public string Name;
        public string About;

        public MaterialInfo(string name, string about)
        {
            Name = name;
            About = about;
        }
    }

    public static class MaterialCatalog
    {
        public static readonly Dictionary<int, MaterialInfo> Materials = new Dictionary<int, MaterialInfo>
        {
            { 0, new MaterialInfo("NONAME", "Неизвестный материал") },
            { 1, new MaterialInfo("", "Сталь3") },
            { 2, new MaterialInfo("09G2S", "09Г2С") },
            { 3, new MaterialInfo("NERJ", "Нержавейка") },
            { 4, new MaterialInfo("RIFL", "Рифл.(Чечевица)") },
            { 5, new MaterialInfo("RIFL_R", "Рифл.(Ромб)") },
            { 6, new MaterialInfo("10HSND", "10ХСНД") }
        };
    }
}
