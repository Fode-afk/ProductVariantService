using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ProductService.Models;

namespace ProductService.Data
{
    /// <summary>
    /// MongoDB implementation of the IProductRepo interface.
    /// Provides CRUD operations for products.
    /// </summary>
    public class ProductRepo(IMongoDatabase database): IProductRepo
    {
        private readonly IMongoCollection<Product> _products = database.GetCollection<Product>("Products");

        public async Task<bool> CreateProductAsync(Product product)
        {
            await _products.InsertOneAsync(product);
            return true;
            //TODO Проверка на выполнение
        }

        public async Task<bool> DeleteProductAsync(string productId)
        {
            await _products.DeleteOneAsync(p => p.ProductId == productId);
            return true;
            //TODO Проверка на выполнение
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

        public async Task<bool> UpdateProductAsync(Product product)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.ProductId, product.ProductId);
            await _products.ReplaceOneAsync(filter, product);
            return true;
            //TODO Проверка на выполнение
        }
    }
}
