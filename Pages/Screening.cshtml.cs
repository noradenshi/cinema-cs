using cinema_cs.Data;
using cinema_cs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace cinema_cs.Pages
{
    [Authorize]
    public class ScreeningModel : PageModel
    {
        private readonly AppDbContext _context;
        public int MaxSeats { get; set; }

        public ScreeningModel(AppDbContext context)
        {
            _context = context;
        }

        public Screening? Screening { get; set; }
        public List<Seat> AllSeats { get; set; } = [];
        public HashSet<int> TakenSeatIds { get; set; } = [];

        [BindProperty]
        public List<int> SelectedSeats { get; set; } = [];

        public decimal PricePerTicket { get; set; } = 31.50m;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Screening = await _context.Screenings
                .Include(s => s.Room)
                .Include(s => s.Tickets!)
                .ThenInclude(t => t.Seat)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (Screening == null) return NotFound();

            AllSeats = await _context.Seats
                .Where(seat => seat.RoomId == Screening.RoomId)
                .OrderBy(seat => seat.Row).ThenBy(seat => seat.Number)
                .ToListAsync();

            TakenSeatIds = Screening.Tickets!.Select(t => t.SeatId).ToHashSet();

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userTicketCount = await _context.Tickets
                .CountAsync(t => t.Screening.Id == Screening.Id && t.OwnerId == userId);

            MaxSeats = Math.Max(0, 8 - userTicketCount);

            // Fetch dynamic price
            var daysAhead = (Screening.Date.Date - DateTime.Now.Date).Days;

            var priceTier = await _context.PriceTiers
                .Where(p => daysAhead >= p.MinDaysBefore && daysAhead <= p.MaxDaysBefore)
                .OrderBy(p => p.Price)
                .FirstOrDefaultAsync();

            PricePerTicket = priceTier!.Price;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Get the screening with current ticket data
            var screening = await _context.Screenings
                .Include(s => s.Tickets)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (screening == null)
                return NotFound();

            // Check if screening has already started
            if (screening.Date <= DateTime.Now)
            {
                TempData["Error"] = "Purchasing tickets is no longer available because this screening has already started.";
                return RedirectToPage("/Index");
            }

            // Get current user ID
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Forbid();

            // Count how many tickets user already owns
            var userTicketCount = await _context.Tickets
                .Where(t => t.ScreeningId == screening.Id && t.OwnerId == userId)
                .CountAsync();

            // Get all already reserved seat IDs
            var takenSeatIds = screening.Tickets?.Select(t => t.SeatId).ToHashSet() ?? new HashSet<int>();
            var seatsToBook = SelectedSeats
                .Where(seatId => !takenSeatIds.Contains(seatId))
                .Take(8)
                .ToList();

            // Check if total exceeds max allowed tickets (8)
            if (userTicketCount + seatsToBook.Count > 8)
            {
                TempData["Error"] = $"You cannot have more than 8 tickets in total. You currently own {userTicketCount} tickets.";
                return RedirectToPage();
            }

            if (!seatsToBook.Any())
            {
                TempData["Error"] = "Selected seats are already taken or invalid.";
                return RedirectToPage(new { id });
            }

            // Determine how many days ahead
            var daysAhead = (screening.Date.Date - DateTime.Now.Date).Days;

            // Fetch dynamic price again
            var priceTier = await _context.PriceTiers
                .Where(p => daysAhead >= p.MinDaysBefore && daysAhead <= p.MaxDaysBefore)
                .OrderBy(p => p.Price)
                .FirstOrDefaultAsync();

            var actualPrice = priceTier!.Price;

            // Create tickets for valid selections
            foreach (var seatId in seatsToBook)
            {
                var ticket = new Ticket
                {
                    ScreeningId = screening.Id,
                    SeatId = seatId,
                    OwnerId = userId,
                    Price = actualPrice
                };

                _context.Tickets.Add(ticket);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Account/Tickets");
        }
    }
}
