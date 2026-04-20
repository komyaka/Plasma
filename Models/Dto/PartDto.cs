namespace Plazma.Models.Dto
{
    public class PartDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int QuantitySummary { get; set; }
        public int QuantityCutted { get; set; }
        public int Shipped { get; set; }
        public string CncId { get; set; }
        public string Thickness { get; set; }
        public string SizeX { get; set; }
        public string SizeY { get; set; }
    }
}
