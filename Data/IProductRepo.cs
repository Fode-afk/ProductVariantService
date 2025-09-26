using ProductService.Models;

namespace ProductService.Data
{
    public interface IProductRepo
    {
        Task<ExecutionResult<string>> CreateProductModelAsync(string ownerId, DateTimeOffset createdAt);                            //Через AUTH удалять токены, если вендор удален
        Task<ExecutionResult<Product>> CreateProductAsync(Product product);                                                         //Через AUTH удалять токены, если вендор удален



        Task<ExecutionResult<Product>> GetProductModelAsync(string productModelId);                                                 //DONE
        Task<ExecutionResult<IEnumerable<Product>>> GetProductsByIdsAsync(string[] productIds);                                     //DONE
        Task<ExecutionResult<IEnumerable<Product>>> GetProductsByOwnerIdAsync(string ownerId);                                      //DONE
        Task<ExecutionResult<IEnumerable<string>>> GetProductsRawByOwnerId(string ownerId);



        Task<ExecutionResult<Product>> UpdateProductAsync(Product product);                                                         //Вынести проверки на Model.IsValidState
        Task<ExecutionResult<Product>> ReassignProduct(string productId, string parentCardId, string ownerId, DateTimeOffset updatedAt);



        Task<ExecutionResult<Product>> ArchiveProductAsync(string productId, string ownerId, DateTimeOffset updatedAt);
        Task<ExecutionResult<Product>> UnarchiveProductAsync(string productId, string ownerId, DateTimeOffset updatedAt);



        Task<ExecutionResult<IEnumerable<Product>>> DeleteProductsAsync(string[] productIds, string ownerId);
        Task<ExecutionResult<IEnumerable<Product>>> DeleteProductsByOwnerIdAsync(string ownerId);



        Task<ExecutionResult<Product>> AddImagesToProductAsync(Product product);
        Task<ExecutionResult> DeleteImagesFromProductAsync(Product product);
    }
}
