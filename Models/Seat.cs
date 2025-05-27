namespace cinema_cs.Models
{
    public class Seat
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        public required string Row { get; set; }

        public int Number { get; set; }

        public required Room Room { get; set; }

        public required ICollection<Ticket> Tickets { get; set; }
    }
}
