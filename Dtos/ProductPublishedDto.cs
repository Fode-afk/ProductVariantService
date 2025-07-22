using ProductService.Models;
using ProductService.Protos;

namespace ProductService.Dtos
{
    public class ProductPublishedDto
    {
        public string ProductId { get; set; }
        public string ParentCardId { get; set; }

        public ProductType Type { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        public bool CanBeOrdered { get; set; }

        public int StockQuantity { get; set; }

        public List<string> ImageURLs { get; set; }

        public List<ProductAttribute> Attributes { get; set; }
    }
}
