using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using ProductService.Models;

namespace ProductService.Data
{
    public class ProductRepo(IMongoDatabase database): IProductRepo
    {
        private readonly IMongoCollection<Product> _products = database.GetCollection<Product>("Products");

        public async Task<bool> CreateProductAsync(Product product)
        {
            try
            {
                await _products.InsertOneAsync(product);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteProductAsync(string productId)
        {
            var res = await _products.DeleteOneAsync(p => p.ProductId == productId);
            return res.DeletedCount > 0;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _products.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductsByOwnerIdAsync(string ownerId)
        {
            return await _products.Find(p => p.OwnerId == ownerId).ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(string productId)
        {
            return await _products.Find(p => p.ProductId == productId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByIdsAsync(string[] productIds)
        {
            return await _products.Find(p => productIds.Contains(p.ProductId)).ToListAsync();
        }

        public async Task<bool> ProductExistsAsync(string productId)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.ProductId, productId);
            return await _products.Find(filter).Limit(1).AnyAsync();
        }

        public async Task<bool> UpdateProductAsync(Product product)
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

            if (product.Attributes != null && product.Attributes.Count != 0)
                updates.Add(updateBuilder.AddToSetEach(p => p.Attributes, product.Attributes));

            if (product.ImageURLs != null && product.ImageURLs.Count != 0)
                updates.Add(updateBuilder.AddToSetEach(p => p.ImageURLs, product.ImageURLs));

            updates.Add(updateBuilder.Set(p => p.UpdatedAt, product.UpdatedAt));

            if (updates.Count == 0)
                return false;

            var update = updateBuilder.Combine(updates);
            var result = await _products.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }


        public async Task<string> GetParentCardIdAsync(string productId)
        {
            var result = await _products.Find(p => p.ProductId == productId).FirstOrDefaultAsync();
            if(result == null)
                return string.Empty;

            return result.ParentCardId;
        }

        public async Task<bool> UpdateParentCardId(Product product)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.ProductId, product.ProductId);

            var updateBuilder = Builders<Product>.Update;
            var updates = new List<UpdateDefinition<Product>>();

            if (!product.ParentCardId.IsNullOrEmpty())
                updates.Add(updateBuilder.Set(p => p.ParentCardId, product.ParentCardId));

            updates.Add(updateBuilder.Set(p => p.UpdatedAt, product.UpdatedAt));

            if (updates.Count == 0)
                return false;

            var update = updateBuilder.Combine(updates);
            var result = await _products.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }
    }
}
