using PMSIMSWebApi.Entities;
using POSIMSWebApi.Dtos.Category;

namespace POSIMSWebApi.Dtos.Product
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<CategoryDto> Categories { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public class CreateOrEditProductDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public List<Guid> ListOfCategoriesId { get; set; }
    }

    public class FilterText : PaginationParams
    {
        public string? Text { get; set; }
    }

    public class PaginationParams : PaginationSorting
    {
        private int _maxItemsPerPage = 50;
        private int itemsPerPage = 10;
        public int Page { get; set; } = 1;
        public int ItemsPerPage
        {
            get => itemsPerPage;
            set => itemsPerPage = value > _maxItemsPerPage ? _maxItemsPerPage : value;
        }
    }

    public class PaginationSorting
    {
        public string? Sorting { get; set; }
    }
}
