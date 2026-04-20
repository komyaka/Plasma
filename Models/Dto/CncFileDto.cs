using System;

namespace Plazma.Models.Dto
{
    public class CncFileDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string OriginalName { get; set; }
        public string Thickness { get; set; }
        public string RealThickness { get; set; }
        public int SheetWidth { get; set; }
        public int Height { get; set; }
        public int Material { get; set; }
        public int Quantity { get; set; }
        public int QuantityDone { get; set; }
        public DateTime AddedTime { get; set; }
    }
}
