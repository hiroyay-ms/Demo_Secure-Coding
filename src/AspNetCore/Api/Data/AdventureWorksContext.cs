using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data
{
    public class AdventureWorksContext : DbContext
    {
        public AdventureWorksContext(DbContextOptions<AdventureWorksContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = "Server=localhost;Database=AdventureWorksLT;User Id=sa;Password=your_password;";
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public DbSet<ProductCategory> ProductCategory => Set<ProductCategory>();
        public DbSet<Product> Product => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCategory>().ToTable("ProductCategory", "SalesLT");
            modelBuilder.Entity<Product>().ToTable("Product", "SalesLT");
        }
    }
}
