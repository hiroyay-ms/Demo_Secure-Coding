using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data
{
    public class AdventureWorksContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public AdventureWorksContext(DbContextOptions<AdventureWorksContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = _configuration.GetValue<string>("SQL_CONNECTION_STRING") ?? throw new InvalidOperationException("Connection string 'SQL_CONNECTION_STRING' not found.");
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
