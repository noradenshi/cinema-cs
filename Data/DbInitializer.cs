using System.Text.Json;
using cinema_cs.Models;
using Microsoft.AspNetCore.Identity;

namespace cinema_cs.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider, AppDbContext context)
        {
            if (context.Movies.Any())
                return; // DB has been seeded

            var json = await System.IO.File.ReadAllTextAsync("Data/Seed/movies.json");
            var movies = JsonSerializer.Deserialize<List<Movie>>(json);

            if (movies == null || movies.Count == 0)
                throw new Exception("No movies found in JSON or deserialization failed.");

            context.Movies.AddRange(movies);

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            var adminRole = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            var adminEmail = "admin@admin";
            var admin = new User
            {
                Name = "Admin",
                Surname = "Adminski",
                Phone = "000000000",
                Email = adminEmail,
                UserName = adminEmail
            };

            var result = await userManager.CreateAsync(admin, "*()890Cs");
            if (!result.Succeeded)
            {
                throw new Exception("Admin creation failed: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await context.SaveChangesAsync();

            if (!await userManager.IsInRoleAsync(admin, adminRole))
            {
                await userManager.AddToRoleAsync(admin, adminRole);
            }

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
            int daysToGenerate = 7;

            for (int dayOffset = 0; dayOffset < daysToGenerate; dayOffset++)
            {
                var currentDate = today.AddDays(dayOffset);

                // Get only premiered movies
                var premieredMovies = movies
                    .Where(m => m.Premiere <= currentDate)
                    .ToList();

                // Dictionary to track scheduled movies by hour (e.g., 13 for 1 PM)
                var scheduledHours = new Dictionary<int, HashSet<int>>();

                foreach (var room in rooms)
                {
                    var currentTime = currentDate.AddHours(9); // reset per room/day
                    var dayEndTime = currentDate.AddHours(24);

                    while (true)
                    {
                        int hour = currentTime.Hour;

                        // Filter out movies already scheduled at this hour
                        var availableMovies = premieredMovies
                            .Where(m => !scheduledHours.TryGetValue(hour, out var ids) || !ids.Contains(m.Id))
                            .ToList();

                        if (!availableMovies.Any())
                            break; // no available movies left for this time slot

                        // Pick one at random
                        var movie = availableMovies[random.Next(availableMovies.Count)];
                        var endTimeWithBreak = currentTime.AddMinutes(movie.Length + 60);

                        if (endTimeWithBreak > dayEndTime)
                            break;

                        // Register movie as scheduled for this hour
                        if (!scheduledHours.ContainsKey(hour))
                            scheduledHours[hour] = new HashSet<int>();
                        scheduledHours[hour].Add(movie.Id);

                        screenings.Add(new Screening
                        {
                            MovieId = movie.Id,
                            RoomId = room.Id,
                            Type = (ScreeningType)random.Next(0, 3),
                            Date = currentTime
                        });

                        currentTime = endTimeWithBreak;
                    }
                }
            }

            context.Screenings.AddRange(screenings);
            await context.SaveChangesAsync();
        }
    }
}
