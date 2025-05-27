namespace cinema_cs.Models
{
    public class Movie
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public required string Description { get; set; }

        public int Length { get; set; } // in minutes

        public DateTime Premiere { get; set; }

        public ICollection<Screening>? Screenings { get; set; }
    }
}
