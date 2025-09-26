using ProductService.Models;

namespace ProductService.Data
{
    public interface ICardRepo
    {
        Task<ExecutionResult<string>> CreateCardAsync(Card card);



        Task<ExecutionResult<IEnumerable<Card>>> GetCardsAsync(string[] cardIds);
        
        
        
        Task<ExecutionResult> ReassignCard(string cardId, string productId, string ownerId, DateTimeOffset updatedAt);
    }
}
