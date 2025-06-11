using ProductService.Models;

namespace ProductService.Data
{
    public interface IProductRepo
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetAllProductsByOwnerIdAsync(string ownerId);

        Task<Product> GetProductByIdAsync(string productId);
        Task<Product> GetProductByIdAndOwnerIdAsync(string productId, string ownerId);

        Task<bool> CreateProductAsync(Product product);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(string productId);
    }
}
