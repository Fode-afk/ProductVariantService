namespace ProductService.Models
{
    /// <summary>
    /// Represents a key-value attribute of a product (e.g., color, size).
    /// </summary>
    public class ProductAttribute
    {
        /// <summary>
        /// Gets or sets the name of the attribute (e.g., "Color").
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the attribute (e.g., "Red").
        /// </summary>
        public string Value { get; set; }
    }
}
