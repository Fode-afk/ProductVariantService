using ProductService.Models;

namespace ProductService.Data
{
    public interface IProductRepo
    {
        Task<ExecutionResult<string>> CreateProductModelAsync(string ownerId, DateTimeOffset createdAt); //Через AUTH удалять токены, если вендор удален
        Task<ExecutionResult<Product>> CreateProductAsync(Product product); //Через AUTH удалять токены, если вендор удален



        Task<ExecutionResult<IEnumerable<Product>>> GetProductsByIdsAsync(string[] productIds);
        Task<ExecutionResult<IEnumerable<Product>>> GetProductsByOwnerIdAsync(string ownerId);



        Task<ExecutionResult<Product>> UpdateProductAsync(Product product);
        Task<ExecutionResult<Product>> UpdateParentCardIdAsync(string productId, string parentCardId, DateTimeOffset updatedAt);



        Task<ExecutionResult<Product>> ArchiveProductAsync(string productId, DateTimeOffset updatedAt);
        Task<ExecutionResult<Product>> UnarchiveProductAsync(string productId, DateTimeOffset updatedAt);



        Task<ExecutionResult<IEnumerable<Product>>> DeleteProductsAsync(string[] productIds);
        Task<ExecutionResult<IEnumerable<Product>>> DeleteProductsByOwnerIdAsync(string ownerId);



        Task<ExecutionResult<Product>> AddImagesToProductAsync(Product product);
        Task<ExecutionResult> DeleteImagesFromProductAsync(Product product);
    }
}
