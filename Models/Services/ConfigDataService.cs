using Plazma.Controllers;
namespace Plazma.Models.Services
{
    public class ConfigDataService
    {
        private readonly PartsClass _parts;

        public ConfigDataService(PartsClass parts)
        {
            _parts = parts;
        }

        public int GetChapterCode(string name)
        {
            return _parts.getChapterCode(name);
        }

        public void ReadMaterials()
        {
            _parts.LoadMaterials();
        }

        public void ReadChapters()
        {
            _parts.LoadChapters();
        }
    }
}
