using System;

namespace Plazma.Models.Dto
{
    public class SheetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Material { get; set; }
        public float Thickness { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Quantity { get; set; }
        public string Owner { get; set; }
        public string Status { get; set; }
        public string Document { get; set; }
        public DateTime Date { get; set; }
    }
}
