using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Kursova.Models;

namespace Kursova.DAL
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameResult> Results { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<GameResult>()
                .HasOne(p => p.User)
                .WithMany(p => p.Results)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<GameResult>()
                .HasOne(p => p.Game)
                .WithMany(p => p.Results)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}