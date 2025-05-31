using cinema_cs.Data;
using cinema_cs.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace cinema_cs.Pages
{
    public class PricingModel : PageModel
    {
        private readonly AppDbContext _context;

        public PricingModel(AppDbContext context)
        {
            _context = context;
        }

        public List<PriceTier> PriceTiers { get; set; } = [];
        public decimal OnlineBookingFee { get; set; } = 1.50m;

        public async Task OnGetAsync()
        {
            PriceTiers = await _context.PriceTiers
                .OrderBy(p => p.Price)
                .ToListAsync();
        }
    }
}
