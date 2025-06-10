using System.ComponentModel.DataAnnotations;

namespace ProductService.Models
{
    public class Product
    {      
        public string Id { get; set; }
       
        public string Name { get; set; }

        public string Description { get; set; }
      
        public int Price { get; set; }

        public List<string> ImageURLs { get; set; }

        public List<ProductAttribute> Attributes { get; set; }
    }
}
