using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using ProductService.Models;

namespace ProductService.Data.Caching
{
    public class CachedProductRepo(IMongoDatabase database, ILogger logger, ICacheRepo cacheRepo) //: IProductRepo
    {
        private readonly IMongoCollection<Product> _products = database.GetCollection<Product>("Products");
        private readonly ILogger _logger = logger;
        private readonly ICacheRepo _cacheRepo = cacheRepo;

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

        public async Task<ExecutionResult<Product>> DeleteProductsAsync(string[] productIds)
        {
            try
            {
                string cacheKey = $"product:{productIds}";

                await _cacheRepo.RemoveAsync(cacheKey);          

                var res = await _products.FindOneAndDeleteAsync(p => productIds.Contains(p.ProductId));              
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

        public async Task<ExecutionResult<Product>> GetProductByIdAsync(string productId)
        {
            try
            {
                string cacheKey = $"product:{productId}";
                var cacheRes = await _cacheRepo.GetManyAsync<Product>([cacheKey], TimeSpan.FromMinutes(10));

                if (cacheRes.success)
                    return new ExecutionResult<Product>(cacheRes.success, cacheRes.message, cacheRes.Value.First());                   

                var res = await _products.Find(p => p.ProductId == productId).FirstOrDefaultAsync();

                if (res == null)
                    return new ExecutionResult<Product>(false, "Product not found", null);

                await _cacheRepo.SetAsync(cacheKey, res, TimeSpan.FromMinutes(10));

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

                var productRes = await _products.Find(p => ids.Contains(p.ProductId)).ToListAsync();

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

        //public async Task<ExecutionResult> ProductExistsAsync(string productId)
        //{
        //    try
        //    {
        //        var filter = Builders<Product>.Filter.Eq(p => p.ProductId, productId);
        //        var res = await _products.Find(filter).Limit(1).AnyAsync();

        //        return new ExecutionResult(res, string.Empty);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Log(ex.Message, LogLevel.Error);
        //        return new ExecutionResult(false, ex.Message);
        //    }
        //}

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


        public async Task<ExecutionResult<Product>> SetCanBeOrderedAsync(string productId, bool canBeOrdered, DateTimeOffset updatedAt)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.ProductId, productId) &
                            Builders<Product>.Filter.Ne(p => p.ParentCardId, "");

                var updateBuilder = Builders<Product>.Update;
                var updates = new List<UpdateDefinition<Product>>();

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

        //public async Task<ExecutionResult<string>> GetParentCardIdAsync(string productId)
        //{
        //    try
        //    {
        //        var result = await _products.Find(p => p.ProductId == productId).FirstOrDefaultAsync();
        //        if (result == null)
        //            return new ExecutionResult<string>(false, "Product not found", string.Empty);

        //        return new ExecutionResult<string>(true, string.Empty, result.ParentCardId);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Log(ex.Message, LogLevel.Error);
        //        return new ExecutionResult<string>(false, ex.Message, string.Empty);
        //    }
        //}

        public async Task<ExecutionResult<Product>> UpdateParentCardIdAsync(Product product)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.ProductId, product.ProductId);

                var updateBuilder = Builders<Product>.Update;
                var updates = new List<UpdateDefinition<Product>>();

                if (product.ParentCardId != null)
                    updates.Add(updateBuilder.Set(p => p.ParentCardId, product.ParentCardId));

                if (product.ParentCardId == string.Empty)
                    updates.Add(updateBuilder.Set(p => p.CanBeOrdered, false));

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

        public async Task<ExecutionResult<Product>> DeleteAttributesFromProductAsync(Product product)
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

        public async Task<ExecutionResult<List<Product>>> DeleteParentCardIdFromProductsAsync(string[] productIds, string cardId, DateTimeOffset updatedAt)
        {
            try
            {
                var filter = Builders<Product>.Filter.In(p => p.ProductId, productIds) &
                             Builders<Product>.Filter.Eq(p => p.ParentCardId, cardId);

                var update = Builders<Product>.Update
                    .Set(p => p.ParentCardId, string.Empty)
                    .Set(p => p.CanBeOrdered, false)
                    .Set(p => p.UpdatedAt, updatedAt);

                var result = await _products.UpdateManyAsync(filter, update);

                var updatedFilter = Builders<Product>.Filter.In(p => p.ProductId, productIds) &
                            Builders<Product>.Filter.Eq(p => p.ParentCardId, string.Empty);

                var updatedProducts = await _products.Find(updatedFilter).ToListAsync();

                if (result.ModifiedCount > 0)
                    foreach (var productId in updatedProducts.Select(p => p.ProductId))
                        await _cacheRepo.RemoveAsync($"product:{productId}");

                return new ExecutionResult<List<Product>>(result.ModifiedCount > 0, string.Empty, updatedProducts);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<List<Product>>(false, ex.Message, null);
            }
        }

        //public async Task<ExecutionResult<string>> GetOwnerIdAsync(string productId)
        //{
        //    try
        //    {
        //        var result = await _products.Find(p => p.ProductId == productId).FirstOrDefaultAsync();

        //        if (result == null)
        //            return new ExecutionResult<string>(false, "Product not found", string.Empty);

        //        return new ExecutionResult<string>(true, string.Empty, result.OwnerId);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Log(ex.Message, LogLevel.Error);
        //        return new ExecutionResult<string>(false, ex.Message, string.Empty);
        //    }
        //}

        public async Task<ExecutionResult<List<Product>>> DeleteProductsByOwnerIdAsync(string ownerId)
        {
            try
            {
                var products = await _products.Find(p => p.OwnerId == ownerId).ToListAsync();
                var res = await _products.DeleteManyAsync(p => p.OwnerId == ownerId);

                if (res != null)
                {
                    foreach (var product in products)
                    {
                        string cacheKey = $"product:{product.ProductId}";
                        await _cacheRepo.RemoveAsync(cacheKey);
                    }
                }

                return new ExecutionResult<List<Product>>(res != null, string.Empty, products);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<List<Product>>(false, ex.Message, null);
            }
        }

        public Task<ExecutionResult<string>> CreateProductModelAsync(string ownerId, DateTimeOffset createdAt)
        {
            throw new NotImplementedException();
        }

        public Task<ExecutionResult<Product>> DeleteProductAsync(string[] productIds)
        {
            throw new NotImplementedException();
        }
    }
}
