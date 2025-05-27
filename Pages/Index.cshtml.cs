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

        // Store the selected date (default to today)
        public DateTime SelectedDate { get; set; } = DateTime.UtcNow.Date;

        public void OnGet(DateTime? date)
        {
            Console.WriteLine(SelectedDate);
            SelectedDate = date?.Date ?? DateTime.UtcNow.Date;

            Movies = _context.Movies
                .Where(m => m.Screenings.Any(s =>
                    s.Date >= SelectedDate && s.Date < SelectedDate.AddDays(1)))
                .Include(m => m.Screenings)
                .ToList();

            foreach (var movie in Movies)
            {
                movie.Screenings = movie.Screenings
                    .Where(s => s.Date.Date == SelectedDate)
                    .ToList();
            }
        }

    };
}
