using cinema_cs.Models;
using cinema_cs.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace cinema_cs.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class TicketsModel : PageModel
    {
        private readonly AppDbContext _context;

        public TicketsModel(AppDbContext context)
        {
            _context = context;
        }

        public class TicketGroup
        {
            public string UserId { get; set; } = string.Empty;
            public User User { get; set; } = default!;
            public int ScreeningId { get; set; }
            public Screening Screening { get; set; } = default!;
            public decimal TotalPrice { get; set; }
            public List<string> SeatNames { get; set; } = new();
        }

        public List<TicketGroup> GroupedTickets { get; set; } = new();

        public List<Movie> Movies { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? EmailFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MovieId { get; set; }  // CHANGED: now an int

        [BindProperty(SupportsGet = true)]
        public DateTime? DateFilter { get; set; }

        public async Task OnGetAsync()
        {
            Movies = await _context.Movies.OrderBy(m => m.Title).ToListAsync();

            var tickets = await _context.Tickets
                .Include(t => t.Owner)
                .Include(t => t.Seat)
                .Include(t => t.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(t => t.Screening)
                    .ThenInclude(s => s.Room)
                .ToListAsync();

            var query = tickets.AsQueryable();

            if (!string.IsNullOrWhiteSpace(EmailFilter))
                query = query.Where(t => t.Owner.Email.Contains(EmailFilter));

            if (MovieId.HasValue)
                query = query.Where(t => t.Screening.MovieId == MovieId.Value);

            if (DateFilter.HasValue)
                query = query.Where(t => t.Screening.Date.Date == DateFilter.Value.Date);

            GroupedTickets = query
                .GroupBy(t => new { t.OwnerId, t.ScreeningId })
                .Select(g => new TicketGroup
                {
                    UserId = g.Key.OwnerId,
                    User = g.First().Owner,
                    ScreeningId = g.Key.ScreeningId,
                    Screening = g.First().Screening,
                    TotalPrice = g.Sum(t => t.Price),
                    SeatNames = g.Select(t => $"{t.Seat.Row}-{t.Seat.Number}").ToList()
                })
                .ToList();
        }

        public async Task<IActionResult> OnPostRefundAsync(string userId, int screeningId)
        {
            var tickets = await _context.Tickets
                .Where(t => t.OwnerId == userId && t.ScreeningId == screeningId)
                .ToListAsync();

            if (tickets.Any())
            {
                _context.Tickets.RemoveRange(tickets);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
