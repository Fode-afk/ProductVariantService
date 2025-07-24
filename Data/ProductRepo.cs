using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using ProductService.Models;

namespace ProductService.Data
{
    public class ProductRepo(IMongoDatabase database, ILogger logger) : IProductRepo
    {
        private readonly IMongoCollection<Product> _products = database.GetCollection<Product>("Products");
        private readonly ILogger _logger = logger;

        public async Task<ExecutionResult> CreateProductAsync(Product product)
        {
            try
            {
                await _products.InsertOneAsync(product);

                return new ExecutionResult(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult(false, ex.Message);
            }
        }

        public async Task<ExecutionResult<Product>> DeleteProductAsync(string productId)
        {
            try
            {
                var res = await _products.FindOneAndDeleteAsync(p => p.ProductId == productId);
                return new ExecutionResult<Product>(res != null, string.Empty, res);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<Product>(false, ex.Message, null);
            }
        }

        public async Task<ExecutionResult<IEnumerable<Product>>> GetAllProductsAsync()
        {
            try
            {
                var res = await _products.Find(_ => true).ToListAsync();
                return new ExecutionResult<IEnumerable<Product>>(res.Count > 0, string.Empty, res);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<IEnumerable<Product>>(false, ex.Message, null);
            }
        }

        public async Task<ExecutionResult<IEnumerable<Product>>> GetAllProductsByOwnerIdAsync(string ownerId)
        {
            try
            {
                var res = await _products.Find(p => p.OwnerId == ownerId).ToListAsync();
                if(res.Count == 0)
                    return new ExecutionResult<IEnumerable<Product>>(false, "Products not found", null);

                return new ExecutionResult<IEnumerable<Product>>(true, string.Empty, res);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<IEnumerable<Product>>(false, ex.Message, null);
            }
        }

        public async Task<ExecutionResult<Product>> GetProductByIdAsync(string productId)
        {
            try
            {
                var res = await _products.Find(p => p.ProductId == productId).FirstOrDefaultAsync();

                if(res == null)
                    return new ExecutionResult<Product>(false, "Product not found", null);

                return new ExecutionResult<Product>(true, string.Empty, res);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<Product>(false, ex.Message, null);
            }
        }

        public async Task<ExecutionResult<IEnumerable<Product>>> GetProductsByIdsAsync(string[] productIds)
        {
            try
            {
                var res = await _products.Find(p => productIds.Contains(p.ProductId)).ToListAsync();

                if (res.Count == 0)
                    return new ExecutionResult<IEnumerable<Product>>(false, "Products not found", null);

                return new ExecutionResult<IEnumerable<Product>>(true, string.Empty, res);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<IEnumerable<Product>>(false, ex.Message, null);
            }
        }

        public async Task<ExecutionResult> ProductExistsAsync(string productId)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.ProductId, productId);
                var res = await _products.Find(filter).Limit(1).AnyAsync();

                return new ExecutionResult(res, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult(false, ex.Message);
            }
        }

        public async Task<ExecutionResult<Product>> UpdateProductAsync(Product product)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.ProductId, product.ProductId);

                var updateBuilder = Builders<Product>.Update;
                var updates = new List<UpdateDefinition<Product>>();

                if (!product.Name.IsNullOrEmpty())
                    updates.Add(updateBuilder.Set(p => p.Name, product.Name));

                if (product.Type != Protos.ProductType.UnknownType)
                    updates.Add(updateBuilder.Set(p => p.Type, product.Type));

                if (product.Price != 0)
                    updates.Add(updateBuilder.Set(p => p.Price, product.Price));

                if (!product.Description.IsNullOrEmpty())
                    updates.Add(updateBuilder.Set(p => p.Description, product.Description));

                if (product.StockQuantity != 0)
                    updates.Add(updateBuilder.Set(p => p.StockQuantity, product.StockQuantity));        
              
                if (updates.Count == 0)
                    return new ExecutionResult<Product>(false, string.Empty, null);

                updates.Add(updateBuilder.Set(p => p.UpdatedAt, product.UpdatedAt));

                var update = updateBuilder.Combine(updates);
                var updatedProduct = await _products.FindOneAndUpdateAsync(
                    filter,
                    update,
                    new FindOneAndUpdateOptions<Product>
                    {
                        ReturnDocument = ReturnDocument.After
                    });

                return new ExecutionResult<Product>(updatedProduct != null, string.Empty, updatedProduct);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<Product>(false, ex.Message, null);
            }
        }


        public async Task<ExecutionResult<Product>> SetCanBeOrderedAsync(string productId, bool canBeOrdered, DateTimeOffset updatedAt)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.ProductId, productId);

                var updateBuilder = Builders<Product>.Update;
                var updates = new List<UpdateDefinition<Product>>();              

                if (!await HasParentCard(productId))
                    return new ExecutionResult<Product>(false, "Product must have a parentCardId", null);

                updates.Add(updateBuilder.Set(p => p.CanBeOrdered, canBeOrdered));

                if (updates.Count == 0)
                    return new ExecutionResult<Product>(false, string.Empty, null);

                updates.Add(updateBuilder.Set(p => p.UpdatedAt, updatedAt));

                var update = updateBuilder.Combine(updates);
                var updatedProduct = await _products.FindOneAndUpdateAsync(
                    filter,
                    update,
                    new FindOneAndUpdateOptions<Product>
                    {
                        ReturnDocument = ReturnDocument.After
                    });

                return new ExecutionResult<Product>(updatedProduct != null, string.Empty, updatedProduct);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<Product>(false, ex.Message, null);
            }
        }

        private async Task<bool> HasParentCard(string productId)
        {           
            var result = await _products.Find(p => p.ProductId == productId && p.ParentCardId != string.Empty).FirstOrDefaultAsync();
            return result != null;
        }

        public async Task<ExecutionResult<string>> GetParentCardIdAsync(string productId)
        {
            try
            {
                var result = await _products.Find(p => p.ProductId == productId).FirstOrDefaultAsync();
                if (result == null)
                    return new ExecutionResult<string>(false, "Product not found", string.Empty);

                return new ExecutionResult<string>(true, string.Empty, result.ParentCardId);

            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<string>(false, ex.Message, string.Empty);
            }
        }

        public async Task<ExecutionResult<Product>> UpdateParentCardIdAsync(Product product)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.ProductId, product.ProductId);

                var updateBuilder = Builders<Product>.Update;
                var updates = new List<UpdateDefinition<Product>>();

                if (product.ParentCardId != null)
                    updates.Add(updateBuilder.Set(p => p.ParentCardId, product.ParentCardId));             

                if (updates.Count == 0)
                    return new ExecutionResult<Product>(false, "Nothing to update", null);

                updates.Add(updateBuilder.Set(p => p.UpdatedAt, product.UpdatedAt));

                var update = updateBuilder.Combine(updates);
                var updatedProduct = await _products.FindOneAndUpdateAsync(
                    filter,
                    update,
                    new FindOneAndUpdateOptions<Product>
                    {
                        ReturnDocument = ReturnDocument.After
                    });

                return new ExecutionResult<Product>(updatedProduct != null, string.Empty, updatedProduct);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<Product>(false, ex.Message, null);
            }            
        }

        public async Task<ExecutionResult<Product>> AddImagesToProductAsync(Product product)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.ProductId, product.ProductId);

                var updateBuilder = Builders<Product>.Update;
                var updates = new List<UpdateDefinition<Product>>();

                if (product.ImageURLs != null && product.ImageURLs.Count != 0)
                    updates.Add(updateBuilder.AddToSetEach(p => p.ImageURLs, product.ImageURLs));

                if (updates.Count == 0)
                    return new ExecutionResult<Product>(false, "Nothing to update", null);

                updates.Add(updateBuilder.Set(p => p.UpdatedAt, product.UpdatedAt));

                var update = updateBuilder.Combine(updates);
                var updatedProduct = await _products.FindOneAndUpdateAsync(
                    filter,
                    update,
                    new FindOneAndUpdateOptions<Product>
                    {
                        ReturnDocument = ReturnDocument.After
                    });

                return new ExecutionResult<Product>(updatedProduct != null, string.Empty, updatedProduct);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<Product>(false, ex.Message, null);
            }   
        }

        public async Task<ExecutionResult> DeleteImagesFromProductAsync(Product product)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.ProductId, product.ProductId);

                var updateBuilder = Builders<Product>.Update;
                var updates = new List<UpdateDefinition<Product>>();

                if (product.ImageURLs != null && product.ImageURLs.Count != 0)
                    updates.Add(updateBuilder.PullAll(p => p.ImageURLs, product.ImageURLs));              

                if (updates.Count == 0)
                    return new ExecutionResult(false, "Nothing to update");

                updates.Add(updateBuilder.Set(p => p.UpdatedAt, product.UpdatedAt));

                var update = updateBuilder.Combine(updates);
                var result = await _products.UpdateOneAsync(filter, update);

                return new ExecutionResult(result.ModifiedCount > 0, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult(false, ex.Message);
            }
        }

        public async Task<ExecutionResult<Product>> AddAttributesToProductAsync(Product product)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.ProductId, product.ProductId);

                var updateBuilder = Builders<Product>.Update;
                var updates = new List<UpdateDefinition<Product>>();

                if (product.Attributes != null && product.Attributes.Count != 0)
                    updates.Add(updateBuilder.AddToSetEach(p => p.Attributes, product.Attributes));

                if (updates.Count == 0)
                    return new ExecutionResult<Product>(false, "Nothing to update", null);

                updates.Add(updateBuilder.Set(p => p.UpdatedAt, product.UpdatedAt));

                var update = updateBuilder.Combine(updates);
                var updatedProduct = await _products.FindOneAndUpdateAsync(
                    filter,
                    update,
                    new FindOneAndUpdateOptions<Product>
                    {
                        ReturnDocument = ReturnDocument.After
                    });

                return new ExecutionResult<Product>(updatedProduct != null, string.Empty, updatedProduct);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<Product>(false, ex.Message, null);
            }
        }

        public async Task<ExecutionResult> DeleteAttributesFromProductAsync(Product product)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.ProductId, product.ProductId);

                var updateBuilder = Builders<Product>.Update;
                var updates = new List<UpdateDefinition<Product>>();

                if (product.Attributes != null && product.Attributes.Count > 0)
                {
                    var keysToRemove = product.Attributes.Select(a => a.Key).ToList();

                    updates.Add(updateBuilder.PullFilter(p => p.Attributes,
                        attr => keysToRemove.Contains(attr.Key)));
                }                

                if (updates.Count == 0)
                    return new ExecutionResult(false, "Nothing to update");

                updates.Add(updateBuilder.Set(p => p.UpdatedAt, product.UpdatedAt));

                var update = updateBuilder.Combine(updates);
                var result = await _products.UpdateOneAsync(filter, update);

                return new ExecutionResult(result.ModifiedCount > 0, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult(false, ex.Message);
            }
        }
    }
}
