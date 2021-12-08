using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kursova.Models;
using Microsoft.EntityFrameworkCore;

namespace Kursova.DAL
{
    public class ProductContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public ProductContext(DbContextOptions<ProductContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.User)
                .WithMany(p => p.purchases);
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Product)
                .WithMany(p => p.purchases);

        }
    }
}