using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using cinema_cs.Models;

namespace cinema_cs.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public UsersModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string? Email { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Name { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Surname { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Phone { get; set; }

        [BindProperty]
        public User User { get; set; } = new() { Name = "", Surname = "", Phone = "" };

        public List<User> Users { get; set; } = new();

        public void OnGet()
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(Email))
                query = query.Where(u => u.NormalizedEmail!.Contains(Email.ToUpper()));
            if (!string.IsNullOrWhiteSpace(Name))
                query = query.Where(u => u.Name!.ToLower().Contains(Name.ToLower()));
            if (!string.IsNullOrWhiteSpace(Surname))
                query = query.Where(u => u.Surname!.ToLower().Contains(Surname.ToLower()));
            if (!string.IsNullOrWhiteSpace(Phone))
                query = query.Where(u => u.Phone!.Contains(Phone));

            Users = query.ToList();
        }

        public async Task<IActionResult> OnPostAddUserAsync(string password)
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _userManager.CreateAsync(User, password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditUserAsync()
        {
            var userToUpdate = await _userManager.FindByIdAsync(User.Id);
            if (userToUpdate == null)
                return NotFound();

            userToUpdate.Email = User.Email;
            userToUpdate.UserName = User.Email;
            userToUpdate.Name = User.Name;
            userToUpdate.Surname = User.Surname;
            userToUpdate.Phone= User.Phone;

            var result = await _userManager.UpdateAsync(userToUpdate);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            return RedirectToPage();
        }
    }
}
