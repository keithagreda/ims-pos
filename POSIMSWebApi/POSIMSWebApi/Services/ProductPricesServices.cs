using PMSIMSWebApi.Entities;
using POSIMSWebApi.Dtos;
using POSIMSWebApi.Dtos.Product;
using POSIMSWebApi.Dtos.ProductPrices;

namespace POSIMSWebApi.Services
{
    public class ProductPricesServices
    {
        private readonly POSIMSDbContext _dbContext;
        public ProductPricesServices(POSIMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //public async Task<ApiResponse<IList<ProductPricesDto>>> GetAllProductPrices(FilterText input)
        //{
        //    var data = _dbContext.ProductPrices.
        //}
    }
}
