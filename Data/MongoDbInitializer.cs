using MongoDB.Driver;
using ProductService.Models;

namespace ProductService.Data
{
    public class MongoDbInitializer
    {
        private readonly IMongoDatabase _database;

        public MongoDbInitializer(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task InitializeAsync()
        {
            var collection = _database.GetCollection<Product>("Products");

            var indexKeysDefinition = Builders<Product>.IndexKeys.Ascending(x => x.ExpiresAt);
            var indexOptions = new CreateIndexOptions { ExpireAfter = TimeSpan.Zero };
            var indexModel = new CreateIndexModel<Product>(indexKeysDefinition, indexOptions);

            try
            {
                await collection.Indexes.CreateOneAsync(indexModel);
            }
            catch (Exception) { }
        }
    }
}