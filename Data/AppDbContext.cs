using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using cinema_cs.Models;

namespace cinema_cs.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Screening> Screenings { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<PriceTier> PriceTiers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PriceTier>().HasData(
                new PriceTier { Id = 1, Label = "Same day", Description = "screening", Price = 31.50m, MinDaysBefore = -1, MaxDaysBefore = 0 },
                new PriceTier { Id = 2, Label = "1–2 days", Description = "ahead", Price = 29.50m, MinDaysBefore = 1, MaxDaysBefore = 2 },
                new PriceTier { Id = 3, Label = "3–4 days", Description = "ahead", Price = 27.50m, MinDaysBefore = 3, MaxDaysBefore = 4 },
                new PriceTier { Id = 4, Label = "5+ days", Description = "ahead", Price = 23.50m, MinDaysBefore = 5, MaxDaysBefore = 8 }
            );

            modelBuilder.Entity<Ticket>().HasKey(t => new { t.ScreeningId, t.SeatId });

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(
                            new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                                v => v.ToUniversalTime(),                           // to DB
                                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)     // from DB
                            )
                        );
                    }
                }
            }
        }
    }
}
