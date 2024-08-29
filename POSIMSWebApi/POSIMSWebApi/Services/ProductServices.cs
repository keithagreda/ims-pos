using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using PMSIMSWebApi.Entities;
using POSIMSWebApi.Dtos;

namespace POSIMSWebApi.Services
{
    public class ProductServices : IProductServices
    {
        private readonly POSIMSDbContext _dbContext;

        public ProductServices(POSIMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<IList<ProductDto>>> GetAll(FilterText filter)
        {
            var data = _dbContext.Products.Select(e => new ProductDto
            {
                Id = e.Id,
                Name = e.Name,
                CreationTime = e.CreationTime
            });

            if (await data.AnyAsync())
            {
                return new ApiResponse<IList<ProductDto>>()
                {
                    Data = new List<ProductDto>(),
                    IsSuccess = false,
                    ErrorMessage = "Products list is empty"
                };
            }

            var pagedSort = data.OrderBy(filter.Sorting ?? "creationtime desc")
                .Skip((filter.Page - 1) * filter.ItemsPerPage)
                .Take(filter.ItemsPerPage);
            var result = await pagedSort.ToListAsync();
            return new ApiResponse<IList<ProductDto>>()
            {
                Data = result,
                IsSuccess = true,
                ErrorMessage = ""
            };
        }

        public async Task<ApiResponse<ProductDto>> GetProductById(Guid productId)
        {
            if (productId == Guid.Empty)
            {
                return new ApiResponse<ProductDto>()
                {
                    Data = new ProductDto(),
                    IsSuccess = false,
                    ErrorMessage = "ProductId is empty"
                };
            }

            var data = await _dbContext
                .Products.Where(e => e.Id == productId)
                .Select(e => new ProductDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    CreationTime = e.CreationTime
                })
                .FirstOrDefaultAsync();

            if (data is null)
            {
                return new ApiResponse<ProductDto>()
                {
                    Data = new ProductDto(),
                    IsSuccess = false,
                    ErrorMessage = "ProductNotFound"
                };
            }

            return new ApiResponse<ProductDto>()
            {
                Data = data,
                IsSuccess = true,
                ErrorMessage = ""
            };
        }

        public async Task<ApiResponse<string>> DeleteProduct(Guid productId)
        {
            if (productId == Guid.Empty)
            {
                return new ApiResponse<string>()
                {
                    Data = "Something went wrong...",
                    IsSuccess = false,
                    ErrorMessage = "ProductId is empty"
                };
            }
            var data = await _dbContext.Products.FirstOrDefaultAsync(e => e.Id == productId);

            if (data is null)
            {
                return new ApiResponse<string>()
                {
                    Data = "Something went wrong...",
                    IsSuccess = false,
                    ErrorMessage = "ProductNotFound"
                };
            }
            var productToBeDeleted = data;
            _dbContext.Products.Remove(productToBeDeleted);
            return new ApiResponse<string>()
            {
                Data = "Successfully removed product!",
                IsSuccess = true,
                ErrorMessage = ""
            };
        }

        public async Task<ApiResponse<ProductDto>> CreateOrEdit(CreateOrEditProductDto input)
        {
            if (input.Id is null)
            {
                //create
                //return ;
                return await CreateProduct(input);
            }
            return await EditProduct(input);
        }

        private async Task<ApiResponse<ProductDto>> CreateProduct(CreateOrEditProductDto input)
        {
            if (input.Id == Guid.Empty)
            {
                return new ApiResponse<ProductDto>()
                {
                    Data = new ProductDto(),
                    IsSuccess = false,
                    ErrorMessage = "ProductIdEmpty"
                };
            }
            Product data = new Product()
            {
                Id = Guid.NewGuid(),
                Name = input.Name,
                Categories = input.Categories,
                CreationTime = DateTime.Now,
                CreatedBy = 1 //temporary
            };

            await _dbContext.Products.AddAsync(data);
            return new ApiResponse<ProductDto>()
            {
                Data = new ProductDto()
                {
                    Id = Guid.NewGuid(),
                    Name = data.Name,
                    Categories = input.Categories,
                    CreationTime = data.CreationTime,
                },
                IsSuccess = true,
                ErrorMessage = ""
            };
        }

        private async Task<ApiResponse<ProductDto>> EditProduct(CreateOrEditProductDto input)
        {
            if (input.Id is null)
            {
                return new ApiResponse<ProductDto>()
                {
                    Data = new ProductDto(),
                    IsSuccess = false,
                    ErrorMessage = "ProductIdNull"
                };
            }

            var data = await GetProductById((Guid)input.Id);

            if (data.IsSuccess is false)
            {
                return new ApiResponse<ProductDto>()
                {
                    Data = data.Data,
                    IsSuccess = false,
                    ErrorMessage = data.ErrorMessage
                };
            }

            Product productToBeEdited = new Product()
            {
                Id = (Guid)input.Id,
                Name = input.Name,
                ModifiedTime = DateTime.Now
            };

            _dbContext.Products.Update(productToBeEdited);
            return new ApiResponse<ProductDto>()
            {
                Data = data.Data,
                IsSuccess = true,
                ErrorMessage = ""
            };
        }
    }
}
