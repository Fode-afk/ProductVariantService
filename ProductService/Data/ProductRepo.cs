using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ProductService.Models;

namespace ProductService.Data
{
    public class ProductRepo(IMongoDatabase database): IProductRepo
    {
        private readonly IMongoCollection<Product> _products = database.GetCollection<Product>("Products");


        public async Task CreateProductAsync(Product product)
        {
            await _products.InsertOneAsync(product);
        }

        public async Task DeleteProductAsync(string productId)
        {
            await _products.DeleteOneAsync(p => p.Id == productId);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _products.Find(_ => true).ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(string productId)
        {
            return await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            //await _products.UpdateOneAsync(product);
        }
    }
}
