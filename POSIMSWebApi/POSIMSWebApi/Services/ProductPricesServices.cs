using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using PMSIMSWebApi.Entities;
using POSIMSWebApi.Dtos;
using POSIMSWebApi.Dtos.Product;
using POSIMSWebApi.Dtos.ProductPrices;
using POSIMSWebApi.Interfaces;
using POSIMSWebApi.QueryExtensions;

namespace POSIMSWebApi.Services
{
    public class ProductPricesServices : IProductPricesServices
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

            var pagedSort = await data.OrderBy(input.Sorting ?? "EffectivityDate desc")
                .Skip((input.Page - 1) * input.ItemsPerPage)
                .Take(input.ItemsPerPage)
                .ToListAsync();

            return new ApiResponse<IList<ProductPricesDto>>()
            {
                Data = pagedSort,
                IsSuccess = true,
                ErrorMessage = ""
            };
        }

        public async Task<ApiResponse<string>> CreateOrEdit(CreateOrEditProductPricesDto input, Guid userId)
        {
            if (input.Price <= 0)
            {
                return new ApiResponse<string>()
                {
                    Data = "Price can't be less than 1",
                    IsSuccess = false,
                    ErrorMessage = "Price Validation Error"
                };
            }

            if (userId == Guid.Empty)
            {
                return new ApiResponse<string>()
                {
                    Data = "Login is required to create a product.",
                    IsSuccess = false,
                    ErrorMessage = "Guid Empty"
                };
            }

            var data = await _dbContext.ProductPrices.FirstOrDefaultAsync(e =>
                e.ProductId == input.ProductId && e.EffectivityDate.Date == DateTime.Now.Date
            );

            if (data is not null)
            {
                return new ApiResponse<string>()
                {
                    Data = await EditProductPrice(input, data, userId),
                    IsSuccess = true,
                    ErrorMessage = ""
                };
            }

            if (input.Id is null)
            {
                return new ApiResponse<string>()
                {
                    Data = await CreateProductPrice(input, userId),
                    IsSuccess = true,
                    ErrorMessage = ""
                };
            }

            return new ApiResponse<string>()
            {
                Data = await CreateProductPrice(input, userId) + " for today.",
                IsSuccess = true,
                ErrorMessage = ""
            };
        }

        private async Task<string> CreateProductPrice(CreateOrEditProductPricesDto input, Guid userId)
        {
            ProductPrice res = new ProductPrice()
            {
                Id = Guid.NewGuid(),
                Price = input.Price,
                ProductId = input.ProductId,
                CreatedBy = userId,
                CreationTime = DateTime.Now,
                IsDeleted = false
            };

            await _dbContext.ProductPrices.AddAsync(res);
            await _dbContext.SaveChangesAsync();
            return "Successfully added product new price";
        }

        private async Task<string> EditProductPrice(
            CreateOrEditProductPricesDto input,
            ProductPrice data,
            Guid userId
        )
        {
            data.ProductId = input.ProductId;
            data.Price = input.Price;
            data.Modifiedby = userId;
            data.ModifiedTime = DateTime.Now;

            await _dbContext.SaveChangesAsync();
            return "Successfully updated product price";
        }
    }
}
