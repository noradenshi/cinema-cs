using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using cinema_cs.Data;
using cinema_cs.Models;

namespace cinema_cs.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;

        public RegisterModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string Name { get; set; } = string.Empty;

            [Required]
            public string Surname { get; set; } = string.Empty;

            [Required]
            [Phone]
            public string Phone { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
            public string Password { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            // Check if email already exists
            if (_context.Users.Any(u => u.Email == Input.Email))
            {
                ModelState.AddModelError(string.Empty, "Email is already registered.");
                return Page();
            }

            // Create new user
            var user = new User
            {
                Name = Input.Name,
                Surname = Input.Surname,
                Phone = Input.Phone,
                Email = Input.Email,
                Password = "",
            };

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, Input.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToPage("/Login");
        }
    }
}
