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
        public DbSet<Product> Products { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.User)
                .WithMany(p => p.purchases)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Product)
                .WithMany(p => p.purchases)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}