namespace cinema_cs.Models
{
    public class Seat
    {
        public int Id { get; set; }

        public required int RoomId { get; set; }

        public required string Row { get; set; }

        public required int Number { get; set; }

        public Room Room { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
