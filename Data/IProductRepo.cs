using ProductService.Models;

namespace ProductService.Data
{
    public interface IProductRepo
    {
        Task<IEnumerable<Product>> GetProductsByIdsAsync(string[] productIds);

        Task<IEnumerable<Product>> GetAllProductsAsync();

        Task<IEnumerable<Product>> GetAllProductsByOwnerIdAsync(string ownerId);

        Task<Product> GetProductByIdAsync(string productId);

        Task<bool> CreateProductAsync(Product product);

        Task<bool> UpdateProductAsync(Product product);

        Task<bool> UpdateParentCardId(Product product);

        Task<bool> DeleteProductAsync(string productId);

        Task<bool> ProductExistsAsync(string productId);

        Task<string> GetParentCardIdAsync(string productId);
    }
}
