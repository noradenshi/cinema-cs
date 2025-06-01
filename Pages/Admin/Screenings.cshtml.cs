using cinema_cs.Models;
using cinema_cs.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace cinema_cs.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ScreeningsModel : PageModel
    {
        private readonly AppDbContext _context;

        public ScreeningsModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime? Date { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MovieId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? RoomId { get; set; }

        public List<Screening> Screenings { get; set; } = new();
        public List<Movie> Movies { get; set; } = new();
        public List<Room> Rooms { get; set; } = new();

        public List<ScreeningSlot> ScreeningSlots { get; set; } = new();

        [BindProperty]
        public Screening Screening { get; set; }

        public async Task OnGetAsync()
        {
            await LoadAsync();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadAsync();
                return Page();
            }

            _context.Screenings.Add(Screening);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadAsync();
                return Page();
            }

            var existing = await _context.Screenings.FindAsync(Screening.Id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.MovieId = Screening.MovieId;
            existing.Date = Screening.Date;
            existing.RoomId = Screening.RoomId;

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var screening = await _context.Screenings.FindAsync(id);
            if (screening != null)
            {
                _context.Screenings.Remove(screening);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        private async Task LoadAsync()
        {
            Movies = await _context.Movies.ToListAsync();
            Rooms = await _context.Rooms.ToListAsync();

            var query = _context.Screenings
                .Include(s => s.Movie)
                .AsQueryable();

            if (Date.HasValue)
            {
                var dateOnly = Date.Value.Date;
                query = query.Where(s => s.Date.ToLocalTime().Date == dateOnly);
            }

            if (MovieId.HasValue)
            {
                query = query.Where(s => s.MovieId == MovieId.Value);
            }

            if (RoomId.HasValue)
            {
                query = query.Where(s => s.RoomId == RoomId.Value);
            }

            Screenings = await query.OrderBy(s => s.Date).ToListAsync();

            ScreeningSlots = Screenings.Select(s => new ScreeningSlot
            {
                RoomId = s.RoomId,
                Start = s.Date.ToLocalTime(),
                End = s.Date.ToLocalTime().AddMinutes(s.Movie.Length)
            }).ToList();
        }

        public class ScreeningSlot
        {
            public int RoomId { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }
    }
}
