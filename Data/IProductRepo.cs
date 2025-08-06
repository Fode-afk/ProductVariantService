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

        Task<ExecutionResult<Product>> UpdateProductAsync(Product product);

        Task<ExecutionResult<Product>> UpdateParentCardIdAsync(Product product);

        Task<ExecutionResult<Product>> SetCanBeOrderedAsync(string productId, bool canBeOrdered, DateTimeOffset updatedAt);

        Task<ExecutionResult<Product>> DeleteProductAsync(string productId);

        Task<ExecutionResult<List<Product>>> DeleteProductsByOwnerIdAsync(string ownerId);

        Task<ExecutionResult> ProductExistsAsync(string productId);

        Task<ExecutionResult<string>> GetParentCardIdAsync(string productId);

        Task<ExecutionResult<string>> GetOwnerIdAsync(string productId);

        Task<ExecutionResult<Product>> AddImagesToProductAsync(Product product);

        Task<ExecutionResult> DeleteImagesFromProductAsync(Product product);

        Task<ExecutionResult<Product>> AddAttributesToProductAsync(Product product);

        Task<ExecutionResult> DeleteAttributesFromProductAsync(Product product);

        Task<ExecutionResult<List<Product>>> DeleteParentCardIdFromProductsAsync(string[] productIds, string cardId, DateTimeOffset updatedAt);
    }
}
