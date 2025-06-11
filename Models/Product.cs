using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductService.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        public string OwnerId { get; set; }
       
        public string Name { get; set; }

        public string Description { get; set; }
      
        public int Price { get; set; }

        public List<string> ImageURLs { get; set; }

        public List<ProductAttribute> Attributes { get; set; }
    }
}
