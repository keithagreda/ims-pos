using PMSIMSWebApi.Entities;
using POSIMSWebApi.Dtos.Product;
using Riok.Mapperly.Abstractions;

namespace POSIMSWebApi.Mapping
{
    [Mapper]
    public partial class ProductMapper
    {
        public partial ProductDto ProductToProductDto(Product product);
    }
}
