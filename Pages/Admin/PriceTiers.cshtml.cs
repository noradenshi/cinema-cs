using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using cinema_cs.Models;
using cinema_cs.Data;
using Microsoft.EntityFrameworkCore;

namespace cinema_cs.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class PriceTiersModel : PageModel
    {
        private readonly AppDbContext _context;

        public PriceTiersModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PriceTier PriceTier { get; set; } = new();

        public List<PriceTier> PriceTiers { get; set; } = new();

        public async Task OnGetAsync()
        {
            PriceTiers = await _context.PriceTiers.ToListAsync();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.PriceTiers.Add(PriceTier);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            var existing = await _context.PriceTiers.FindAsync(PriceTier.Id);
            if (existing == null) return NotFound();

            existing.Label = PriceTier.Label;
            existing.Description = PriceTier.Description;
            existing.Price = PriceTier.Price;
            existing.MinDaysBefore = PriceTier.MinDaysBefore;
            existing.MaxDaysBefore = PriceTier.MaxDaysBefore;

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var toDelete = await _context.PriceTiers.FindAsync(id);
            if (toDelete == null) return NotFound();

            _context.PriceTiers.Remove(toDelete);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
