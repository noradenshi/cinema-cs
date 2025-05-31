using System.Text.Json;
using cinema_cs.Models;

namespace cinema_cs.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (context.Movies.Any())
                return; // DB has been seeded

            var json = await System.IO.File.ReadAllTextAsync("Data/Seed/movies.json");
            var movies = JsonSerializer.Deserialize<List<Movie>>(json);

            if (movies == null || movies.Count == 0)
                throw new Exception("No movies found in JSON or deserialization failed.");

            context.Movies.AddRange(movies);

            var rooms = new List<Room> { new(), new(), new() };
            context.Rooms.AddRange(rooms);
            await context.SaveChangesAsync();

            var seats = new List<Seat>();
            foreach (var room in rooms)
            {
                for (char row = 'A'; row <= 'D'; row++)
                {
                    for (int number = 1; number <= 10; number++)
                    {
                        seats.Add(new Seat
                        {
                            RoomId = room.Id,
                            Row = row.ToString(),
                            Number = number
                        });
                    }
                }
            }
            context.Seats.AddRange(seats);
            await context.SaveChangesAsync();

            var screenings = new List<Screening>();
            var random = new Random();
            var today = DateTime.Today;
            int daysToGenerate = 7; // adjust as needed

            for (int dayOffset = 0; dayOffset < daysToGenerate; dayOffset++)
            {
                var currentDate = today.AddDays(dayOffset);

                // Get only premiered movies
                var premieredMovies = movies
                    .Where(m => m.Premiere <= currentDate.Date)
                    .ToList();

                foreach (var room in rooms)
                {
                    var currentTime = currentDate.AddHours(10); // start at 10:00

                    while (currentTime.Hour < 22)
                    {
                        // Pick a random movie that fits
                        var movie = premieredMovies[random.Next(premieredMovies.Count)];

                        // Screening end time with 1 hour break
                        var nextAvailableTime = currentTime.AddMinutes(movie.Length + 60);

                        // If next screening would exceed 22:00, break
                        if (nextAvailableTime.Hour >= 22 && nextAvailableTime.Minute > 0)
                            break;

                        screenings.Add(new Screening
                        {
                            MovieId = movie.Id,
                            RoomId = room.Id,
                            Type = (ScreeningType)random.Next(0, 3), // Original, Dubbed, Subtitled
                            Date = currentTime
                        });

                        currentTime = nextAvailableTime;
                    }
                }
            }

            context.Screenings.AddRange(screenings);
            await context.SaveChangesAsync();
        }
    }
}
