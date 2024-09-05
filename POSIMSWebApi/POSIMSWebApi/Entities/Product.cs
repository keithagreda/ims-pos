using System.ComponentModel.DataAnnotations.Schema;
using POSIMSWebApi.Interfaces;

namespace PMSIMSWebApi.Entities
{
    public class Product : FullyAudited
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        //nav prop
        public ICollection<Category> Categories { get; set; }
    }

    public class Category : FullyAudited
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        //nav prop
        public ICollection<Product> Products { get; set; }
    }

    public class ProductPrice : FullyAudited
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public DateTime EffectivityDate { get; set; }
        public Guid ProductId { get; set; }

        [ForeignKey("ProductId")]
        //nav prop
        public Product ProductFK { get; set; }
    }

    public class ProductSale : FullyAudited
    {
        public Guid Id { get; set; }
        public DateTime TimeSold { get; set; }
        public decimal ActualSellingPrice { get; set; }
        public string SoldTo { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalProductSales { get; set; }
        public Guid ProductPriceId { get; set; }

        [ForeignKey("ProductPriceId")]
        //nav prop
        public ProductPrice ProductPriceFk { get; set; }
    }

    public class FullyAudited : ISoftDelete
    {
        public Guid CreatedBy { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? Modifiedby { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
    }
}
