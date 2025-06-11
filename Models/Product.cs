using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductService.Models
{
    /// <summary>
    /// Represents a product in the system.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Gets or sets the unique identifier of the product.
        /// Stored as an ObjectId in MongoDB.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the product owner (user who created the product).
        /// </summary>
        public string OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// Gets or sets a list of image URLs associated with the product.
        /// </summary>
        public List<string> ImageURLs { get; set; }

        /// <summary>
        /// Gets or sets the list of attributes describing product details (e.g., color, size).
        /// </summary>
        public List<ProductAttribute> Attributes { get; set; }
    }
}
