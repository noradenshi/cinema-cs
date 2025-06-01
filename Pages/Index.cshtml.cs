using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using cinema_cs.Data;
using cinema_cs.Models;

namespace cinema_cs.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Movie> Movies { get; set; } = new();

        public DateTime SelectedDate { get; set; }

        public void OnGet(DateTime? date)
        {
            SelectedDate = (date?.Date ?? DateTime.Today);

            var dayStart = SelectedDate.Date;
            var dayEnd = dayStart.AddDays(1);

            Movies = _context.Movies
                .Where(m => m.Screenings.Any(s => s.Date.ToLocalTime() >= dayStart && s.Date.ToLocalTime() < dayEnd))
                .Include(m => m.Screenings
                    .Where(s => s.Date.ToLocalTime() >= dayStart && s.Date.ToLocalTime() < dayEnd))
                .ToList();

            foreach (var movie in Movies)
            {
                movie.Screenings = movie.Screenings
                    .OrderBy(s => s.Date)
                    .ToList();
            }

            var allScreeningsToday = _context.Screenings
                .Where(s => s.Date.ToLocalTime() >= dayStart && s.Date.ToLocalTime() < dayEnd)
                .ToList();
        }
    }
}
