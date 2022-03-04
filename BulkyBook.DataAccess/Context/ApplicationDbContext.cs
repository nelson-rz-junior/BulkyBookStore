#nullable disable
using BulkyBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Context
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<CoverType> CoverTypes { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Company> Companies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().Property(p => p.ListPrice).HasColumnType("decimal(7,2)");
            modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(7,2)");
            modelBuilder.Entity<Product>().Property(p => p.Price50).HasColumnType("decimal(7,2)");
            modelBuilder.Entity<Product>().Property(p => p.Price100).HasColumnType("decimal(7,2)");

            base.OnModelCreating(modelBuilder);
        }
    }
}
