namespace cinema_cs.Models
{
    public enum ScreeningType
    {
        Original,
        Dubbed,
        Subtitled
    }

    public class Screening
    {
        public int Id { get; set; }

        public int MovieId { get; set; }

        public int RoomId { get; set; }

        public ScreeningType Type { get; set; }

        public DateTime Date { get; set; }

        public Movie? Movie { get; set; }

        public Room? Room { get; set; }

        public ICollection<Ticket>? Tickets { get; set; }
    }
}
