using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace cinema_cs.Models
{
    public class User : IdentityUser
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Surname { get; set; }

        [Required]
        public required string Phone { get; set; }

        public ICollection<Ticket>? Tickets { get; set; }
    }
}
