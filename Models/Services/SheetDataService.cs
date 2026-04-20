using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Plazma.Controllers;
using Plazma.Models.Dto;

namespace Plazma.Models.Services
{
    public class SheetDataService
    {
        private readonly PartsClass _parts;

        public SheetDataService(PartsClass parts)
        {
            _parts = parts;
        }

        public void ReadSheets(string query, params SqlParameter[] parameters)
        {
            _parts.ReadSheets(query, parameters);
        }

        public void AddSheet(PartsClass._sheet sheet)
        {
            _parts.AddSheettoBD(sheet);
        }

        public int DeleteSheetById(int id)
        {
            return _parts.FreeRequestToBD("DELETE FROM SHEETS WHERE ID=@id", new SqlParameter("@id", id));
        }

        public List<SheetDto> GetSheetDtos()
        {
            return _parts.Sheets.Select(ToDto).ToList();
        }

        public static SheetDto ToDto(PartsClass._sheet sheet)
        {
            return new SheetDto
            {
                Id = sheet.Id,
                Name = sheet.Name,
                Material = sheet.Matherial,
                Thickness = sheet.Tickness,
                Width = sheet.Width,
                Height = sheet.Heigth,
                Quantity = sheet.Quantity,
                Owner = sheet.Owner,
                Status = sheet.Status,
                Document = sheet.Document,
                Date = sheet.Date
            };
        }
    }
}
