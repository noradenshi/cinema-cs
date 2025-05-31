using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cinema_cs.Models
{
    public class Ticket
    {
        [Key, Column(Order = 0)]
        public int ScreeningId { get; set; }

        [Key, Column(Order = 1)]
        public int SeatId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string OwnerId { get; set; } = default!;

        public Screening Screening { get; set; }

        public Seat Seat { get; set; }

        public User Owner { get; set; }
    }
}
