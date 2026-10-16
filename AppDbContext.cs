using Microsoft.EntityFrameworkCore;
using EventManagementAPI.Models;

namespace EventManagementAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Venue> Venues => Set<Venue>();
        public DbSet<Enrolment> Enrolments => Set<Enrolment>(); // Participate entity
        public DbSet<Result> Results => Set<Result>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Category>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<Venue>().HasIndex(v => v.Name).IsUnique();

            // Relationships - satisfies "2 records administrators", "3 records events", "many participants in event/category", "a participant has many results"
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Venue)
                .WithMany(v => v.Events)
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Enrolment>()
                .HasOne(en => en.Event)
                .WithMany(e => e.Enrolments)
                .HasForeignKey(en => en.EventId);

            modelBuilder.Entity<Enrolment>()
                .HasOne(en => en.User)
                .WithMany(u => u.Enrolments)
                .HasForeignKey(en => en.UserId);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrolment)
                .WithMany(e => e.Results)
                .HasForeignKey(r => r.EnrolmentId);

            // Seed Roles via enum, no table needed, but seed categories/venues for demo
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Under 20", AgeGroup = "Under 20", Distance = "10km" },
                new Category { Id = 2, Name = "Senior", AgeGroup = "Senior", Distance = "21km" }
            );
        }
    }
}
