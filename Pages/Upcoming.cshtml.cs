using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using cinema_cs.Data;
using cinema_cs.Models;

namespace cinema_cs.Pages
{
    public class UpcomingModel : PageModel
    {
        private readonly AppDbContext _context;

        public UpcomingModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Movie> UpcomingMovies { get; set; }

        public async Task OnGetAsync()
        {
            var oneWeekFromNow = DateTime.UtcNow.AddDays(7);
            UpcomingMovies = await _context.Movies
                .Where(m => m.Premiere > oneWeekFromNow)
                .OrderBy(m => m.Premiere)
                .ToListAsync();
        }
    }
}
