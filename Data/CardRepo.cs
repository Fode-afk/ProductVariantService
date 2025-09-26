using MongoDB.Driver;
using ProductService.Models;
using ZstdSharp.Unsafe;

namespace ProductService.Data
{
    public class CardRepo(IMongoDatabase database, IConfiguration configuration, ILogger logger) : ICardRepo
    {
        private readonly IMongoCollection<Card> _cards = database.GetCollection<Card>("Cards");

        private readonly IConfiguration _config = configuration;
        private readonly ILogger _logger = logger;

        public async Task<ExecutionResult<string>> CreateCardAsync(Card card)
        {
            try
            {
                var res = await _cards.Find(p => p.ProductIds.Any(id => card.ProductIds.Contains(id))).ToListAsync();

                if (res != null) 
                {
                    return new ExecutionResult<string>(false, "Product already assigned to the card", null);
                }



                await _cards.InsertOneAsync(card);
                return new ExecutionResult<string>(true, string.Empty, card.CardId);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<string>(false, ex.Message, string.Empty);
            }
        }

        
        
        public async Task<ExecutionResult<IEnumerable<Card>>> GetCardsAsync(string[] cardIds)
        {
            try
            {
                var res = await _cards.Find(p => cardIds.Contains(p.CardId)).ToListAsync();

                return new ExecutionResult<IEnumerable<Card>>(res != null, string.Empty, res);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult<IEnumerable<Card>>(false, ex.Message, null);
            }
        }

        
        
        public async Task<ExecutionResult> ReassignCard(string cardId, string productId, string ownerId, DateTimeOffset updatedAt)
        {
            try
            {
                var filter = Builders<Card>.Filter.Eq(p => p.CardId, cardId) & 
                             Builders<Card>.Filter.Nin(productId, p => p.ProductIds) & 
                             Builders<Card>.Filter.Eq(p => p.OwnerId, ownerId);

                var updateBuilder = Builders<Card>.Update;
                var updates = new List<UpdateDefinition<Card>>();

                updates.Add(updateBuilder.AddToSet(p => p.ProductIds, productId));
                
                var update = updateBuilder.Combine(updates);

                var res = await _cards.FindOneAndUpdateAsync(filter, update, 
                    new FindOneAndUpdateOptions<Card>
                    {
                        ReturnDocument = ReturnDocument.After
                    });

                return new ExecutionResult(res != null, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogLevel.Error);
                return new ExecutionResult(false, ex.Message);
            }
        }
    }
}
