using Microsoft.EntityFrameworkCore;
using PMSIMSWebApi.Entities;
using POSIMSWebApi.Dtos;
using POSIMSWebApi.Dtos.Product;
using POSIMSWebApi.Dtos.ProductPrices;
using POSIMSWebApi.QueryExtensions;

namespace POSIMSWebApi.Services
{
    public class ProductPricesServices
    {
        private readonly POSIMSDbContext _dbContext;

        public ProductPricesServices(POSIMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<IList<ProductPricesDto>>> GetAllProductPrices(
            FilterText input
        )
        {
            var data = _dbContext
                .ProductPrices.Include(e => e.ProductFK)
                .WhereIf(
                    !string.IsNullOrWhiteSpace(input.Text),
                    e => e.ProductFK.Name == input.Text
                )
                .Select(e => new ProductPricesDto
                {
                    Id = e.Id,
                    Price = e.Price,
                    ProductName = e.ProductFK.Name,
                    EffectivityDate = e.EffectivityDate,
                });

            if (!await data.AnyAsync()) { }
        }
    }
}
