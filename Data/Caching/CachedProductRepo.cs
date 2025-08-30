using ProductService.Models;

namespace ProductService.Data.Caching
{
    public class CachedProductRepo : IProductRepo
    {
        public Task<ExecutionResult<Product>> AddAttributesToProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult<Product>> AddImagesToProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult> CreateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult<Product>> DeleteAttributesFromProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult> DeleteImagesFromProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult<List<Product>>> DeleteParentCardIdFromProductsAsync(string[] productIds, string cardId, DateTimeOffset updatedAt)
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult<Product>> DeleteProductAsync(string productId)
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult<List<Product>>> DeleteProductsByOwnerIdAsync(string ownerId)
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult<IEnumerable<Product>>> GetAllProductsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult<IEnumerable<Product>>> GetAllProductsByOwnerIdAsync(string ownerId)
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult<string>> GetOwnerIdAsync(string productId)
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult<string>> GetParentCardIdAsync(string productId)
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult<Product>> GetProductByIdAsync(string productId)
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult<IEnumerable<Product>>> GetProductsByIdsAsync(string[] productIds)
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult> ProductExistsAsync(string productId)
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult<Product>> SetCanBeOrderedAsync(string productId, bool canBeOrdered, DateTimeOffset updatedAt)
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult<Product>> UpdateParentCardIdAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult<Product>> UpdateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
