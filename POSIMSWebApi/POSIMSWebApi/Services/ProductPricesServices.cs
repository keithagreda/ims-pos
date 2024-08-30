using Microsoft.EntityFrameworkCore;
using PMSIMSWebApi.Entities;
using POSIMSWebApi.Dtos;
using POSIMSWebApi.Dtos.Product;
using POSIMSWebApi.Dtos.ProductPrices;
using POSIMSWebApi.QueryExtensions;
using System.Linq.Dynamic.Core;

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

            if (!await data.AnyAsync()) 
            {
                return new ApiResponse<IList<ProductPricesDto>>()
                {
                    Data = new List<ProductPricesDto>(),
                    IsSuccess = false,
                    ErrorMessage = "ProductPricesNotFound"
                };
            }

            var pagedSort = await data.OrderBy(input.Sorting ?? "CreationTime desc")
                .Skip((input.Page - 1) * input.ItemsPerPage)
                .Take(input.ItemsPerPage).ToListAsync();

            return new ApiResponse<IList<ProductPricesDto>>()
            {
                Data = pagedSort,
                IsSuccess = true,
                ErrorMessage = ""
            };
        }

        public async Task<ApiResponse<string>> CreateOrEdit(CreateOrEditProductPricesDto input)
        {

        }

        private async Task<string> CreateProductPrices(CreateOrEditProductPricesDto input)
        {
            var data = await _dbContext.ProductPrices.Where(e => e.ProductId == input.ProductId)
        }
    }
}
