using System.Linq.Dynamic.Core;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PMSIMSWebApi.Entities;
using POSIMSWebApi.Auth;
using POSIMSWebApi.Dtos;
using POSIMSWebApi.Dtos.Category;
using POSIMSWebApi.Dtos.Product;
using POSIMSWebApi.Interfaces;
using POSIMSWebApi.Mapping;
using POSIMSWebApi.QueryExtensions;

namespace POSIMSWebApi.Services
{
    public class ProductServices : IProductServices
    {
        private readonly POSIMSDbContext _dbContext;
        private readonly ProductMapper _productMapper;
        private readonly UserManager<ApplicationIdentityUser> _userManager;

        public ProductServices(POSIMSDbContext dbContext, ProductMapper productMapper, UserManager<ApplicationIdentityUser> userManager)
        {
            _dbContext = dbContext;
            _productMapper = productMapper;
            _userManager = userManager;
        }


        public async Task<ApiResponse<IList<ProductDto>>> GetAll(FilterText filter)
        {
            var data = _dbContext
                .Products.Include(e => e.Categories)
                .Select(e => new ProductDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Categories = e
                        .Categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
                        .ToList(),
                    CreationTime = e.CreationTime
                })
                .WhereIf(
                    !string.IsNullOrWhiteSpace(filter.Text),
                    p => p.Name.Contains(filter.Text)
                );

            if (!await data.AnyAsync())
            {
                return new ApiResponse<IList<ProductDto>>()
                {
                    Data = new List<ProductDto>(),
                    IsSuccess = false,
                    ErrorMessage = "Products list is empty"
                };
            }

            var pagedSort = data.OrderBy(filter.Sorting ?? "CreationTime desc")
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

        public async Task<ApiResponse<ProductDto>> CreateOrEdit(CreateOrEditProductDto input, Guid currentUserId)
        {
            if(currentUserId == Guid.Empty)
            {
                return new ApiResponse<ProductDto>()
                {
                    Data = new ProductDto(),
                    IsSuccess = false,
                    ErrorMessage = "Login is required to create a product."
                };
            }

            if (input.Id is null)
            {
                return await CreateProduct(input, currentUserId);
            }
            return await EditProduct(input, currentUserId);
        }

        private async Task<ApiResponse<ProductDto>> CreateProduct(CreateOrEditProductDto input, Guid currentUserId)
        {
            try
            {
                if (input.Id == Guid.Empty)
                {
                    return new ApiResponse<ProductDto>()
                    {
                        Data = new ProductDto(),
                        IsSuccess = false,
                        ErrorMessage = "ProductIdIsSetToEmpty"
                    };
                }

                // Fetch existing categories from the database
                var existingCategories = _dbContext.Categories.Where(c =>
                    input.ListOfCategoriesId.Contains(c.Id)
                );
                // Create the new product
                var product = new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = input.Name,
                    CreationTime = DateTime.Now,
                    CreatedBy = currentUserId,
                    Categories = await existingCategories.ToListAsync(), // Use existing categories
                    IsDeleted = false
                };

                await _dbContext.Products.AddAsync(product);
                await _dbContext.SaveChangesAsync();

                // Prepare the response
                return new ApiResponse<ProductDto>()
                {
                    Data = new ProductDto()
                    {
                        Id = product.Id, // Use the actual ID of the created product
                        Name = product.Name,
                        Categories = await existingCategories
                            .Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
                            .ToListAsync(),
                        CreationTime = product.CreationTime,
                    },
                    IsSuccess = true,
                    ErrorMessage = ""
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductDto>()
                {
                    Data = new ProductDto(),
                    IsSuccess = false,
                    ErrorMessage = ex.InnerException.Message.ToString()
                };
            }
        }

        private async Task<ApiResponse<ProductDto>> EditProduct(CreateOrEditProductDto input, Guid currentUserId)
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
                ModifiedTime = DateTime.Now,
                Modifiedby = currentUserId
            };

            _dbContext.Products.Update(productToBeEdited);
            await _dbContext.SaveChangesAsync();
            return new ApiResponse<ProductDto>()
            {
                Data = data.Data,
                IsSuccess = true,
                ErrorMessage = ""
            };
        }
    }
}
