using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Plazma.Controllers;
using Plazma.Models.Dto;

namespace Plazma.Models.Services
{
    public class PartDataService
    {
        private readonly PartsClass _parts;

        public PartDataService(PartsClass parts)
        {
            _parts = parts;
        }

        public void ReadParts(string query = "Select* from parts where ARHIVE is null order by id", params SqlParameter[] parameters)
        {
            _parts.ReadParts(query, parameters);
        }

        public List<string> ChangeStatus(int id, int quantDone = -1)
        {
            return _parts.changestatus(id, quantDone);
        }

        public void SetQuantity(int id, int quantity)
        {
            _parts.SetQuantity(id, quantity);
        }

        public List<PartDto> GetPartDtos()
        {
            return _parts.Parts.Select(ToDto).ToList();
        }

        public static PartDto ToDto(PartsClass._Part part)
        {
            return new PartDto
            {
                Id = part.Id,
                Name = part.Name,
                Quantity = part.Quantity,
                QuantitySummary = part.QuantitySummary,
                QuantityCutted = part.QuantityCutted,
                Shipped = part.Shipped,
                CncId = part.CNCID,
                Thickness = part.tickness,
                SizeX = part.Size_X,
                SizeY = part.Size_Y
            };
        }
    }
}
