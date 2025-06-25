using ProductService.Models;
using ProductService.Protos;

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

        Task<bool> AddImagesToProductAsync(Product product);

        Task<bool> DeleteImagesFromProductAsync(Product product);

        Task<bool> AddAttributesToProductAsync(Product product);

        Task<bool> DeleteAttributesFromProductAsync(Product product);
    }
}
