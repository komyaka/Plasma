using Plazma.Controllers;
namespace Plazma.Models.Services
{
    public class ShipmentDataService
    {
        private readonly PartsClass _parts;

        public ShipmentDataService(PartsClass parts)
        {
            _parts = parts;
        }

        public void Ship(string orderName)
        {
            _parts.Ship(orderName);
        }
    }
}
