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

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public DbSet<OrderHeader> OrderHeaders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().Property(p => p.ListPrice).HasColumnType("decimal(12,2)");
            modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(12,2)");
            modelBuilder.Entity<Product>().Property(p => p.Price50).HasColumnType("decimal(12,2)");
            modelBuilder.Entity<Product>().Property(p => p.Price100).HasColumnType("decimal(12,2)");

            modelBuilder.Entity<ShoppingCart>().Property(p => p.UnitPrice).HasColumnType("decimal(12,2)");
            modelBuilder.Entity<ShoppingCart>().Property(p => p.FinalPrice).HasColumnType("decimal(12,2)");

            modelBuilder.Entity<OrderHeader>().Property(p => p.OrderTotal).HasColumnType("decimal(12,2)");

            modelBuilder.Entity<OrderDetail>().Property(p => p.UnitPrice).HasColumnType("decimal(12,2)");
            modelBuilder.Entity<OrderDetail>().Property(p => p.FinalPrice).HasColumnType("decimal(12,2)");

            base.OnModelCreating(modelBuilder);
        }
    }
}
