using Microsoft.EntityFrameworkCore;
using POSIMSWebApi.Interceptors;

namespace PMSIMSWebApi.Entities
{
    public class POSIMSDbContext : DbContext
    {
        private readonly SoftDeleteInterceptor _softDeleteInterceptor;

        public POSIMSDbContext(
            DbContextOptions<POSIMSDbContext> options,
            SoftDeleteInterceptor softDeleteInterceptor
        )
            : base(options)
        {
            _softDeleteInterceptor = softDeleteInterceptor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_softDeleteInterceptor);
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<ProductSale> ProductSales { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Automatically adding query filter to
            // all LINQ queries that use Movie
            modelBuilder.Entity<Product>().HasQueryFilter(x => x.IsDeleted == false);
            modelBuilder.Entity<ProductPrice>().HasQueryFilter(x => x.IsDeleted == false);
            modelBuilder.Entity<ProductSale>().HasQueryFilter(x => x.IsDeleted == false);
            modelBuilder.Entity<Category>().HasQueryFilter(x => x.IsDeleted == false);
        }
    }
}
