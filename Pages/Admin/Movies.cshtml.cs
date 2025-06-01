using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using cinema_cs.Data;
using cinema_cs.Models;

namespace cinema_cs.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class MoviesModel : PageModel
    {
        private readonly AppDbContext _context;

        public MoviesModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Movie> Movies { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? OrderBy { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SortDirection { get; set; } = "asc";

        public async Task OnGetAsync()
        {
            var query = _context.Movies.AsQueryable();

            bool isDescending = SortDirection?.ToLower() == "desc";

            query = OrderBy?.ToLower() switch
            {
                "title" => isDescending ? query.OrderByDescending(m => m.Title) : query.OrderBy(m => m.Title),
                "premiere" => isDescending ? query.OrderByDescending(m => m.Premiere) : query.OrderBy(m => m.Premiere),
                _ => query.OrderBy(m => m.Premiere) // Default sort
            };

            Movies = await query.ToListAsync();
        }

        public async Task<IActionResult> OnPostAddAsync(Movie Movie)
        {
            if (!ModelState.IsValid)
                return RedirectToPage();

            _context.Movies.Add(Movie);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync(Movie Movie)
        {
            if (!ModelState.IsValid)
                return RedirectToPage();

            var movieInDb = await _context.Movies.FindAsync(Movie.Id);
            if (movieInDb == null)
                return NotFound();

            movieInDb.Title = Movie.Title;
            movieInDb.Description = Movie.Description;
            movieInDb.Length = Movie.Length;
            movieInDb.Premiere = Movie.Premiere;

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
