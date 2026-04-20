using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plazma.Models
{
    public class Sheet
    {
        public Sheet()
        {
            Width = 0;
            Heigth = 0;
            Tickness = 0;
            Material = 0;
        }
        public Sheet(float tikness,int material,int width,int heigth)
        {
            Width = width;
            Heigth = heigth;
            Tickness = tikness;
            Material = material;
        }

        public int Width { get; set; }
        public int Heigth { get; set; }
        public float Tickness { get; set; }
        public int Material { get; set; }

    }
}