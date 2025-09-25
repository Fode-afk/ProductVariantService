using MongoDB.Driver;
using ProductService.Models;

namespace ProductService.Data.Caching
{
    public class CachedProductRepo(IMongoDatabase database, ILogger logger, ICacheRepo cacheRepo) : IProductRepo
    {
        private readonly IMongoCollection<Product> _products = database.GetCollection<Product>("Products");
        private readonly ILogger _logger = logger;
        private readonly ICacheRepo _cacheRepo = cacheRepo;  
        private const int pageSize = 50;

        public async Task<ExecutionResult<string>> CreateProductModelAsync(string ownerId, DateTimeOffset createdAt)
        {
            try
            {
                var product = new Product()
                {
                    OwnerId = ownerId,
                    CreatedAt = createdAt,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10)
                };

                await _products.InsertOneAsync(product);

                return new ExecutionResult<string>(true, string.Empty, product.ProductId);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<string>(false, ex.Message, string.Empty);
            }
        }

        public async Task<ExecutionResult<Product>> CreateProductAsync(Product product)
        {
            try
            {
                var existingProduct = await _products.Find(p => p.ProductId == product.ProductId && p.ExpiresAt != null).FirstOrDefaultAsync();

                if (existingProduct == null)
                {
                    return new ExecutionResult<Product>(false, "Temp product not found. Must create temp product model first.", null);
                }

                var filter = Builders<Product>.Filter.Eq(p => p.ProductId, product.ProductId);
                var updateBuilder = Builders<Product>.Update;
                var updates = new List<UpdateDefinition<Product>>();

                updates.Add(updateBuilder.Set(p => p.Name, product.Name));
                updates.Add(updateBuilder.Set(p => p.Type, product.Type));
                updates.Add(updateBuilder.Set(p => p.Price, product.Price));
                updates.Add(updateBuilder.Set(p => p.Description, product.Description));
                updates.Add(updateBuilder.Set(p => p.StockQuantity, product.StockQuantity));
                updates.Add(updateBuilder.Set(p => p.ParentCardId, product.ParentCardId));
                updates.Add(updateBuilder.Set(p => p.CanBeOrdered, product.CanBeOrdered));
                updates.Add(updateBuilder.Set(p => p.ImageURLs, product.ImageURLs));
                updates.Add(updateBuilder.Set(p => p.Attributes, product.Attributes));
                updates.Add(updateBuilder.Set(p => p.UpdatedAt, product.UpdatedAt));

                updates.Add(updateBuilder.Set(p => p.ExpiresAt, (DateTime?)null));

                var update = updateBuilder.Combine(updates);
                var result = await _products.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Product>
                {
                    ReturnDocument = ReturnDocument.After
                });

                if (result == null)
                {
                    return new ExecutionResult<Product>(false, "Failed to create product or product was already processed.", null);
                }

                return new ExecutionResult<Product>(true, string.Empty, result);
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
                var cacheKeys = productIds.Select(id => $"product:{id}").ToArray();
                var cacheRes = await _cacheRepo.GetManyAsync<Product>(cacheKeys, TimeSpan.FromMinutes(10));

                var ids = productIds.ToList();

                if (cacheRes.success)
                {
                    foreach (var cachedId in cacheRes.Value.Select(p => p!.ProductId))
                        ids.Remove(cachedId);

                    if (ids.Count == 0)
                    {
                        return new ExecutionResult<IEnumerable<Product>>(cacheRes.success, cacheRes.message, cacheRes.Value.OfType<Product>());
                    }
                }

                var productRes = await _products.Find(p => productIds.Contains(p.ProductId) && p.ExpiresAt == null).ToListAsync();

                if (productRes.Count == 0)
                    return new ExecutionResult<IEnumerable<Product>>(false, "Products not found", null);

                var res = cacheRes.success ? productRes.Concat(cacheRes.Value.OfType<Product>()) : productRes;

                if (!productIds.OrderBy(x => x).SequenceEqual(res.Select(p => p.ProductId).OrderBy(x => x)))
                    return new ExecutionResult<IEnumerable<Product>>(false, "Not all products were found", null);

                foreach (var product in productRes)
                {
                    await _cacheRepo.SetAsync($"product:{product.ProductId}", product, TimeSpan.FromMinutes(10));
                }

                return new ExecutionResult<IEnumerable<Product>>(true, string.Empty, res);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<IEnumerable<Product>>(false, ex.Message, null);
            }
        }

        public async Task<ExecutionResult<(IEnumerable<Product>, int)>> GetProductsByOwnerIdAsync(string ownerId, int pageNumber)
        {
            try
            {
                if (pageNumber <= 0) 
                    pageNumber = 1;              

                var filter = Builders<Product>.Filter.Eq(p => p.OwnerId, ownerId) &
                             Builders<Product>.Filter.Eq(p => p.ExpiresAt, null);

                var totalCount = await _products.CountDocumentsAsync(filter);
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                if (pageNumber > totalPages)
                    pageNumber = totalPages;

                var skip = (pageNumber - 1) * pageSize;

                if (totalCount == 0)
                    return new ExecutionResult<(IEnumerable<Product>, int)>(false, "Products not found", (null, 0));
            
                var res = await _products
                    .Find(filter)
                    .Skip(skip)
                    .Limit(pageSize)
                    .ToListAsync();

                return new ExecutionResult<(IEnumerable<Product>, int)>(true, string.Empty, (res, totalPages));
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<(IEnumerable<Product>, int)>(false, ex.Message, (null, 0));
            }
        }

        public async Task<ExecutionResult<Product>> UpdateProductAsync(Product product)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.ProductId, product.ProductId) &
                Builders<Product>.Filter.Eq(p => p.ExpiresAt, null);

                var existingProduct = await _products.Find(filter).FirstOrDefaultAsync();
                if (existingProduct == null)
                    return new ExecutionResult<Product>(false, "Product not found", null);



                if (product.Price < 0)
                    return new ExecutionResult<Product>(false, "Price cannot be negative", null);

                if (product.StockQuantity < 0)
                    return new ExecutionResult<Product>(false, "Stock quantity cannot be negative", null);

                if (string.IsNullOrWhiteSpace(product.Name))
                    return new ExecutionResult<Product>(false, "Name cannot be empty", null);

                if (string.IsNullOrWhiteSpace(product.Description))
                    return new ExecutionResult<Product>(false, "Description cannot be empty", null);



                var updateBuilder = Builders<Product>.Update;
                var updates = new List<UpdateDefinition<Product>>();

                if (product.Name != existingProduct.Name)
                    updates.Add(updateBuilder.Set(p => p.Name, product.Name));

                if (product.Type != existingProduct.Type)
                    updates.Add(updateBuilder.Set(p => p.Type, product.Type));

                if (product.Price != existingProduct.Price)
                    updates.Add(updateBuilder.Set(p => p.Price, product.Price));

                if (product.Description != existingProduct.Description)
                    updates.Add(updateBuilder.Set(p => p.Description, product.Description));

                if (product.StockQuantity != existingProduct.StockQuantity)
                    updates.Add(updateBuilder.Set(p => p.StockQuantity, product.StockQuantity));

                if (product.ParentCardId != existingProduct.ParentCardId)
                    updates.Add(updateBuilder.Set(p => p.ParentCardId, product.ParentCardId));

                if (!AreListsEqual(product.Attributes, existingProduct.Attributes))
                    updates.Add(updateBuilder.Set(p => p.Attributes, product.Attributes));

                if (updates.Count == 0)
                    return new ExecutionResult<Product>(false, "No changes detected", existingProduct);

                updates.Add(updateBuilder.Set(p => p.UpdatedAt, product.UpdatedAt));

                var update = updateBuilder.Combine(updates);
                var updatedProduct = await _products.FindOneAndUpdateAsync(
                    filter,
                    update,
                    new FindOneAndUpdateOptions<Product>
                    {
                        ReturnDocument = ReturnDocument.After
                    });

                if (updatedProduct != null)
                    await _cacheRepo.RemoveAsync($"product:{updatedProduct.ProductId}");

                return new ExecutionResult<Product>(updatedProduct != null, string.Empty, updatedProduct);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<Product>(false, ex.Message, null);
            }
        }

        public async Task<ExecutionResult<Product>> UpdateParentCardIdAsync(string productId, string parentCardId, DateTimeOffset updatedAt)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.ProductId, productId) &
                Builders<Product>.Filter.Eq(p => p.ExpiresAt, null);

                var existingProduct = await _products.Find(filter).FirstOrDefaultAsync();
                if (existingProduct == null)
                    return new ExecutionResult<Product>(false, "Product not found", null);

                var updateBuilder = Builders<Product>.Update;
                var updates = new List<UpdateDefinition<Product>>();

                if (parentCardId != existingProduct.ParentCardId)
                    updates.Add(updateBuilder.Set(p => p.ParentCardId, parentCardId));

                if (updates.Count == 0)
                    return new ExecutionResult<Product>(false, "Nothing to update", null);

                updates.Add(updateBuilder.Set(p => p.UpdatedAt, updatedAt));

                var update = updateBuilder.Combine(updates);
                var updatedProduct = await _products.FindOneAndUpdateAsync(
                    filter,
                    update,
                    new FindOneAndUpdateOptions<Product>
                    {
                        ReturnDocument = ReturnDocument.After
                    });

                if (updatedProduct != null)
                    await _cacheRepo.RemoveAsync($"product:{updatedProduct.ProductId}");

                return new ExecutionResult<Product>(updatedProduct != null, string.Empty, updatedProduct);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<Product>(false, ex.Message, null);
            }
        }

        public async Task<ExecutionResult<Product>> ArchiveProductAsync(string productId, DateTimeOffset updatedAt)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.ProductId, productId) &
                                Builders<Product>.Filter.Eq(p => p.ExpiresAt, null);

                var updateBuilder = Builders<Product>.Update;
                var updates = new List<UpdateDefinition<Product>>();

                updates.Add(updateBuilder.Set(p => p.IsArchived, true));

                updates.Add(updateBuilder.Set(p => p.UpdatedAt, updatedAt));

                var update = updateBuilder.Combine(updates);
                var updatedProduct = await _products.FindOneAndUpdateAsync(
                    filter,
                    update,
                    new FindOneAndUpdateOptions<Product>
                    {
                        ReturnDocument = ReturnDocument.After
                    });

                if (updatedProduct != null)
                    await _cacheRepo.RemoveAsync($"product:{updatedProduct.ProductId}");

                return new ExecutionResult<Product>(updatedProduct != null, string.Empty, updatedProduct);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<Product>(false, ex.Message, null);
            }
        }

        public async Task<ExecutionResult<Product>> UnarchiveProductAsync(string productId, DateTimeOffset updatedAt)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.ProductId, productId) &
                                Builders<Product>.Filter.Eq(p => p.ExpiresAt, null);

                var updateBuilder = Builders<Product>.Update;
                var updates = new List<UpdateDefinition<Product>>();

                updates.Add(updateBuilder.Set(p => p.IsArchived, false));

                updates.Add(updateBuilder.Set(p => p.UpdatedAt, updatedAt));

                var update = updateBuilder.Combine(updates);
                var updatedProduct = await _products.FindOneAndUpdateAsync(
                    filter,
                    update,
                    new FindOneAndUpdateOptions<Product>
                    {
                        ReturnDocument = ReturnDocument.After
                    });

                if (updatedProduct != null)
                    await _cacheRepo.RemoveAsync($"product:{updatedProduct.ProductId}");

                return new ExecutionResult<Product>(updatedProduct != null, string.Empty, updatedProduct);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<Product>(false, ex.Message, null);
            }
        }

        public async Task<ExecutionResult<IEnumerable<Product>>> DeleteProductsAsync(string[] productIds)
        {
            try
            {
                var res = await _products.Find(p => productIds.Contains(p.ProductId) && p.ExpiresAt == null).ToListAsync();
                var deleteRes = await _products.DeleteManyAsync(p => productIds.Contains(p.ProductId) && p.ExpiresAt == null);

                if (deleteRes.DeletedCount > 0)
                {
                    foreach (var id in productIds)
                    {
                        string cacheKey = $"product:{id}";
                        await _cacheRepo.RemoveAsync(cacheKey);
                    }
                }

                return new ExecutionResult<IEnumerable<Product>>(deleteRes.DeletedCount > 0, string.Empty, res);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<IEnumerable<Product>>(false, ex.Message, null);
            }
        }

        public async Task<ExecutionResult<IEnumerable<Product>>> DeleteProductsByOwnerIdAsync(string ownerId)
        {
            try
            {
                var products = await _products.FindAsync(p => p.OwnerId == ownerId);
                var res = await _products.DeleteManyAsync(p => p.OwnerId == ownerId);

                if (res != null || res.DeletedCount > 0)
                {
                    foreach (var product in products.ToEnumerable())
                    {
                        string cacheKey = $"product:{product.ProductId}";
                        await _cacheRepo.RemoveAsync(cacheKey);
                    }
                }

                return new ExecutionResult<IEnumerable<Product>>(res.DeletedCount > 0, string.Empty, products.ToList());
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<IEnumerable<Product>>(false, ex.Message, null);
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

                if (updatedProduct != null)
                    await _cacheRepo.RemoveAsync($"product:{updatedProduct.ProductId}");

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

                if (result.ModifiedCount > 0)
                    await _cacheRepo.RemoveAsync($"product:{product.ProductId}");

                return new ExecutionResult(result.ModifiedCount > 0, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult(false, ex.Message);
            }
        }

        private bool AreListsEqual<T>(List<T>? list1, List<T>? list2)
        {
            if (list1 == null && list2 == null) return true;
            if (list1 == null || list2 == null) return false;
            if (list1.Count != list2.Count) return false;

            for (int i = 0; i < list1.Count; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(list1[i], list2[i]))
                    return false;
            }
            return true;
        }

        public async Task<ExecutionResult<Product>> SetCanBeOrderedAsync(string productId, bool canBeOrdered, DateTimeOffset updatedAt)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.ProductId, productId) &
                                Builders<Product>.Filter.Eq(p => p.ExpiresAt, null);

                var updateBuilder = Builders<Product>.Update;
                var updates = new List<UpdateDefinition<Product>>();

                if (!await HasParentCard(productId))
                    return new ExecutionResult<Product>(false, "Product must have an assigned card", null);

                updates.Add(updateBuilder.Set(p => p.CanBeOrdered, canBeOrdered));

                updates.Add(updateBuilder.Set(p => p.UpdatedAt, updatedAt));

                var update = updateBuilder.Combine(updates);
                var updatedProduct = await _products.FindOneAndUpdateAsync(
                    filter,
                    update,
                    new FindOneAndUpdateOptions<Product>
                    {
                        ReturnDocument = ReturnDocument.After
                    });

                if (updatedProduct != null)
                    await _cacheRepo.RemoveAsync($"product:{updatedProduct.ProductId}");

                return new ExecutionResult<Product>(updatedProduct != null, string.Empty, updatedProduct);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<Product>(false, ex.Message, null);
            }
        }

        private async Task<bool> HasParentCard(string productId) //TOREFACTOR
        {
            var result = await _products.Find(p => p.ProductId == productId && p.ParentCardId != string.Empty && p.ExpiresAt == null).FirstOrDefaultAsync();
            return result != null;
        }
    }
}