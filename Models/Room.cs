namespace cinema_cs.Models
{
    public class Room
    {
        public int Id { get; set; }

        public ICollection<Seat>? Seats { get; set; }

        public ICollection<Screening>? Screenings { get; set; }
    }
}
