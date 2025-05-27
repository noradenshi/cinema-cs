using cinema_cs.Models;

namespace cinema_cs.Data
{
    public static class DbInitializer
    {
        private static string GenerateMovieTitle(Random rand)
        {
            var adjectives = new[] { "Silent", "Dark", "Lost", "Final", "Hidden", "Broken", "Furious", "Eternal" };
            var nouns = new[] { "Empire", "Journey", "Truth", "Shadow", "Destiny", "Secret", "Horizon", "Code" };
            var suffixes = new[] { "", "", "", ": Reloaded", ": Redemption", ": The Beginning", ": Awakening" };

            var title = $"{adjectives[rand.Next(adjectives.Length)]} {nouns[rand.Next(nouns.Length)]}{suffixes[rand.Next(suffixes.Length)]}";
            return title;
        }

        public static void Seed(AppDbContext context)
        {
            if (context.Movies.Any())
                return; // DB has been seeded

            var rng = new Random();

            // --- ROOMS ---
            var rooms = new List<Room>
            {
                new() { Capacity = 100 },
                new() { Capacity = 80 },
                new() { Capacity = 60 }
            };
            context.Rooms.AddRange(rooms);
            context.SaveChanges();

            // --- ACTIVE MOVIES (10 released this week or earlier) ---
            var now = DateTime.UtcNow;
            var activeMovies = new List<Movie>();

            for (int i = 0; i < 10; i++)
            {
                var premiere = now.AddDays(-rng.Next(0, 7)); // within the past week
                var movie = new Movie
                {
                    Title = GenerateMovieTitle(rng),
                    Description = $"Description for Movie {i + 1}",
                    Length = rng.Next(90, 150),
                    Premiere = premiere,
                    Screenings = new List<Screening>()
                };

                // 14 days of screenings
                for (int dayOffset = 0; dayOffset < 14; dayOffset++)
                {
                    int screeningsToday = rng.Next(3, 6); // 3-5 per day
                    for (int j = 0; j < screeningsToday; j++)
                    {
                        var time = now.Date.AddDays(dayOffset).AddHours(10 + j * 2); // 10:00, 12:00, etc.
                        movie.Screenings.Add(new Screening
                        {
                            Date = time,
                            Type = (ScreeningType)rng.Next(0, 3),
                            Room = rooms[rng.Next(rooms.Count)],
                        });
                    }
                }

                activeMovies.Add(movie);
            }
            context.Movies.AddRange(activeMovies);

            // --- UPCOMING MOVIES (10 releasing > 1 week from now but this year) ---
            var upcomingMovies = new List<Movie>();

            for (int i = 0; i < 10; i++)
            {
                var futureDate = now.AddDays(rng.Next(8, 180)); // More than a week away, this year
                upcomingMovies.Add(new Movie
                {
                    Title = GenerateMovieTitle(rng),
                    Description = $"Upcoming release {i + 1}",
                    Length = rng.Next(90, 150),
                    Premiere = futureDate,
                    Screenings = new List<Screening>()
                });
            }
            context.Movies.AddRange(upcomingMovies);

            context.SaveChanges();
        }
    }
}
