using ProductService.Models;

namespace ProductService.Data
{
    /// <summary>
    /// Interface for product data access operations.
    /// Provides methods for retrieving, creating, updating, and deleting products.
    /// </summary>
    public interface IProductRepo
    {
        /// <summary>
        /// Retrieves all products from the database.
        /// </summary>
        /// <returns>A collection of all products.</returns>
        Task<IEnumerable<Product>> GetAllProductsAsync();

        /// <summary>
        /// Retrieves all products that belong to a specific owner.
        /// </summary>
        /// <param name="ownerId">The ID of the owner.</param>
        /// <returns>A collection of products associated with the specified owner.</returns>
        Task<IEnumerable<Product>> GetAllProductsByOwnerIdAsync(string ownerId);

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <returns>The product if found; otherwise, null.</returns>
        Task<Product> GetProductByIdAsync(string productId);

        /// <summary>
        /// Creates a new product in the database.
        /// </summary>
        /// <param name="product">The product to be created.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        Task<bool> CreateProductAsync(Product product);

        /// <summary>
        /// Updates an existing product in the database.
        /// </summary>
        /// <param name="product">The product with updated values.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        Task<bool> UpdateProductAsync(Product product);

        /// <summary>
        /// Deletes a product from the database by its ID.
        /// </summary>
        /// <param name="productId">The ID of the product to delete.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        Task<bool> DeleteProductAsync(string productId);
    }
}
