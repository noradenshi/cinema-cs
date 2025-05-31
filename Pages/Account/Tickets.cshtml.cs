using cinema_cs.Models;
using cinema_cs.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace cinema_cs.Pages.Account
{
    [Authorize]
    public class TicketsModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public TicketsModel(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public List<Ticket> ActiveTickets { get; set; } = [];
        public List<Ticket> ArchivedTickets { get; set; } = [];

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            var now = DateTime.Now;

            var tickets = await _context.Tickets
                .Include(t => t.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(t => t.Screening)
                    .ThenInclude(s => s.Room)
                .Include(t => t.Seat)
                .Where(t => t.OwnerId == user.Id)
                .OrderBy(t => t.Screening.Date)
                .ToListAsync();

            ActiveTickets = tickets.Where(t => t.Screening.Date > now).ToList();
            ArchivedTickets = tickets.Where(t => t.Screening.Date <= now).ToList();
        }

        public async Task<IActionResult> OnPostRefundAsync(int screeningId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var tickets = await _context.Tickets
                .Where(t => t.ScreeningId == screeningId && t.OwnerId == userId && t.Screening.Date > DateTime.UtcNow)
                .ToListAsync();

            if (!tickets.Any()) {
                TempData["Error"] = "Request denied. The screening has likely already started.";
                return RedirectToPage();
            }

            _context.Tickets.RemoveRange(tickets);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

    }
}
