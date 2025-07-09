using ProductService.Models;

namespace ProductService.Data
{
    public interface IProductRepo
    {
        Task<ExecutionResult<IEnumerable<Product>>> GetProductsByIdsAsync(string[] productIds);

        Task<ExecutionResult<IEnumerable<Product>>> GetAllProductsAsync();

        Task<ExecutionResult<IEnumerable<Product>>> GetAllProductsByOwnerIdAsync(string ownerId);

        Task<ExecutionResult<Product>> GetProductByIdAsync(string productId);

        Task<ExecutionResult> CreateProductAsync(Product product);

        Task<ExecutionResult> UpdateProductAsync(Product product);

        Task<ExecutionResult> UpdateParentCardId(Product product);

        Task<ExecutionResult> DeleteProductAsync(string productId);

        Task<ExecutionResult> ProductExistsAsync(string productId);

        Task<ExecutionResult<string>> GetParentCardIdAsync(string productId);

        Task<ExecutionResult> AddImagesToProductAsync(Product product);

        Task<ExecutionResult> DeleteImagesFromProductAsync(Product product);

        Task<ExecutionResult> AddAttributesToProductAsync(Product product);

        Task<ExecutionResult> DeleteAttributesFromProductAsync(Product product);
    }
}
