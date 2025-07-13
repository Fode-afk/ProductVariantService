using ProductService.EventProcessing;
using ProductService.Models;
using ProductService.Protos;

namespace ProductService.Dtos
{
    public class ProductPublishedDto
    {
        public EventType Event { get; set; }
        public ServicesEnum Service { get; set; }

        public string ProductId { get; set; }
        public string ParentCardId { get; set; }

        public ProductType Type { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public int Price { get; set; }

        public int StockQuantity { get; set; }

        public List<string> ImageURLs { get; set; }

        public List<ProductAttribute> Attributes { get; set; }
    }
}
