using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plazma.Models
{
    public class Part
    {
        public int Id;
        public string Name;
            public int Quantity;
            public int QuantitySummary;
            public int QuantityCutted;
            public string CNCID;
            public float tickness;
            public int Size_X;
            public int Size_Y;
            public string Reserve1;
            public string Reserve2;
            public string Reserve3;
            public Part(string name="", int quantity = 1, int quantitysum = 1, int Quantitycutted = 0, float Tickness = 0, int SizeX = 0, int SizeY = 0, string CncId = "", string reserve1 = "", string reserve2 = "", string reserve3 = "")
            {
                Id = 0;
                Name = name;
                Quantity = quantity;
                QuantitySummary = quantitysum;
                QuantityCutted = Quantitycutted;
                tickness = Tickness;
                Size_X = SizeX;
                Size_Y = SizeY;
                CNCID = CncId;
                Reserve1 = reserve1;
                Reserve2 = reserve2;
                Reserve3 = reserve3;
            }

        
    }
}