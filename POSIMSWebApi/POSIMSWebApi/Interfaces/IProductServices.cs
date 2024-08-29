using POSIMSWebApi.Dtos;
using POSIMSWebApi.Dtos.Product;

namespace POSIMSWebApi.Interfaces
{
    public interface IProductServices
    {
        Task<ApiResponse<ProductDto>> CreateOrEdit(CreateOrEditProductDto input);
        Task<ApiResponse<string>> DeleteProduct(Guid productId);
        Task<ApiResponse<IList<ProductDto>>> GetAll(FilterText filter);
        Task<ApiResponse<ProductDto>> GetProductById(Guid productId);
    }
}
