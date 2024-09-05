using POSIMSWebApi.Dtos;
using POSIMSWebApi.Dtos.Product;
using POSIMSWebApi.Dtos.ProductPrices;

namespace POSIMSWebApi.Interfaces
{
    public interface IProductPricesServices
    {
        Task<ApiResponse<string>> CreateOrEdit(CreateOrEditProductPricesDto input, Guid userId);
        Task<ApiResponse<IList<ProductPricesDto>>> GetAllProductPrices(FilterText input);
    }
}
