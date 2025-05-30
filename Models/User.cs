using System.ComponentModel.DataAnnotations;

namespace cinema_cs.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Surname { get; set; }

        public required string Phone { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; } // Hashed password

        public ICollection<Ticket>? Tickets { get; set; }
    }
}
