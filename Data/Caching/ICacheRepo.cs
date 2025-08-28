using StackExchange.Redis;

namespace ProductService.Data.Caching
{
    public interface ICacheRepo
    {
        Task<ExecutionResult<List<T?>>> GetManyAsync<T>(string[] keys);
        Task<ExecutionResult> SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<ExecutionResult> RemoveAsync(string key);
    }
}
