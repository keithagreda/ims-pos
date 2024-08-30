using System.ComponentModel.DataAnnotations.Schema;
using POSIMSWebApi.Dtos.Product;

namespace POSIMSWebApi.Dtos.ProductPrices
{
    public class ProductPricesDto
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public DateTime EffectivityDate { get; set; }
        public string ProductName { get; set; }
    }

    public class CreateOrEditProductPricesDto
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public DateTime EffectivityDate { get; set; }
        public Guid ProductId { get; set; }
    }
}
