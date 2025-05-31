namespace cinema_cs.Models
{
    public class PriceTier
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int MinDaysBefore { get; set; }
        public int MaxDaysBefore { get; set; }
    }
}
