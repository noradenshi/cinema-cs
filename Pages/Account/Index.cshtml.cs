using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using cinema_cs.Models;

namespace cinema_cs.Pages.Account
{
    [Authorize]
    public class AccountModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountModel(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public string? StatusMessage { get; set; }

        [BindProperty]
        public PasswordModel PasswordInput { get; set; } = default!;

        [BindProperty]
        [DataType(DataType.Password)]
        public string DeletePassword { get; set; } = string.Empty;

        public class InputModel
        {
            [Required]
            public string Name { get; set; } = string.Empty;

            [Required]
            public string Surname { get; set; } = string.Empty;

            [Required]
            [Phone]
            public string Phone { get; set; } = string.Empty;

            public string Email { get; set; } = string.Empty;
        }

        public class PasswordModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string CurrentPassword { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
            public string NewPassword { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            Input = new InputModel
            {
                Name = user.Name,
                Surname = user.Surname,
                Phone = user.Phone,
                Email = user.Email!
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove(nameof(PasswordInput.CurrentPassword));
            ModelState.Remove(nameof(PasswordInput.NewPassword));
            ModelState.Remove(nameof(PasswordInput.ConfirmPassword));
            ModelState.Remove(nameof(DeletePassword));

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            Input.Email = user.Email!;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            user.Name = Input.Name;
            user.Surname = Input.Surname;
            user.Phone = Input.Phone;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                StatusMessage = "Your profile has been updated.";
                return RedirectToPage();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            ModelState.Remove(nameof(Input.Name));
            ModelState.Remove(nameof(Input.Surname));
            ModelState.Remove(nameof(Input.Email));
            ModelState.Remove(nameof(Input.Phone));
            ModelState.Remove(nameof(DeletePassword));

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            Input.Name = user.Name;
            Input.Surname = user.Surname;
            Input.Phone = user.Phone;
            Input.Email = user.Email!;

            if (!ModelState.IsValid)
                return Page();

            var result = await _userManager.ChangePasswordAsync(user, PasswordInput.CurrentPassword, PasswordInput.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your password has been changed.";
                return RedirectToPage();
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            ModelState.Remove(nameof(Input.Name));
            ModelState.Remove(nameof(Input.Surname));
            ModelState.Remove(nameof(Input.Email));
            ModelState.Remove(nameof(Input.Phone));
            ModelState.Remove(nameof(PasswordInput.CurrentPassword));
            ModelState.Remove(nameof(PasswordInput.NewPassword));
            ModelState.Remove(nameof(PasswordInput.ConfirmPassword));

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            Input.Name = user.Name;
            Input.Surname = user.Surname;
            Input.Phone = user.Phone;
            Input.Email = user.Email!;

            if (!ModelState.IsValid)
                return Page();

            if (!await _userManager.CheckPasswordAsync(user, DeletePassword))
            {
                ModelState.AddModelError(nameof(DeletePassword), "Incorrect password.");
                return Page();
            }

            await _signInManager.SignOutAsync();
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return Page();
            }

            return RedirectToPage("/Index");
        }
    }
}
