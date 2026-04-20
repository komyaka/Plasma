using System;
using System.Collections.Generic;
using System.Linq;
using Plazma.Controllers;
using Plazma.Models.Dto;

namespace Plazma.Models.Services
{
    public class CncDataService
    {
        private readonly PartsClass _parts;

        public CncDataService(PartsClass parts)
        {
            _parts = parts;
        }

        public void ReadCnc(string query)
        {
            _parts.readCNC(query);
        }

        public int AddCncToBd(PartsClass._CNC cncRecord)
        {
            return _parts.AddCNCtoBD(cncRecord);
        }

        public int AddCncToBd(string fileName, string originalFile, int runtimeOneSheet = 0, int quantity = 1, int quantityDone = 1, DateTime? addedTime = null, DateTime? fileCreatedTime = null, string realTickness = "0", string tickness = "0", string sheets = "", string reserve1 = "", string reserve2 = "", string reserve3 = "")
        {
            return _parts.AddCNCtoBD(fileName, originalFile, runtimeOneSheet, quantity, quantityDone, addedTime, fileCreatedTime, realTickness, tickness, sheets, reserve1, reserve2, reserve3);
        }

        public void DeleteCnc(int id)
        {
            _parts.DeleteCNC(id);
        }

        public List<CncFileDto> GetCncFileDtos()
        {
            return _parts.CNCs.Select(ToDto).ToList();
        }

        public static CncFileDto ToDto(PartsClass._CNC cnc)
        {
            return new CncFileDto
            {
                Id = cnc.Id,
                FileName = cnc.FileName,
                OriginalName = cnc.OriginalName,
                Thickness = cnc.tickness,
                RealThickness = cnc.realrickness,
                SheetWidth = cnc.SheetWidth,
                Height = cnc.SheetHeigh,
                Material = cnc.Material,
                Quantity = cnc.Quantity,
                QuantityDone = cnc.QuantityDone,
                AddedTime = cnc.AddedTime
            };
        }
    }
}
