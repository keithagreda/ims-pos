using Microsoft.EntityFrameworkCore;

namespace PMSIMSWebApi.Entities
{
    public class POSIMSDbContext : DbContext
    {
        public POSIMSDbContext(DbContextOptions<POSIMSDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<ProductSale> ProductSales { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
