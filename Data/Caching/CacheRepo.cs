using StackExchange.Redis;
using System.Text.Json;

namespace ProductService.Data.Caching
{
    public class CacheRepo(IConnectionMultiplexer connectionMultiplexer) : ICacheRepo
    {
        private readonly IDatabase _db = connectionMultiplexer.GetDatabase();

        public async Task<ExecutionResult<T?>> GetAsync<T>(string key)
        {
            try
            {
                var value = await _db.StringGetAsync(key);

                if (value.IsNullOrEmpty)
                    return new ExecutionResult<T?>(false, string.Empty, default);

                return new ExecutionResult<T?>(true, string.Empty, JsonSerializer.Deserialize<T>(value!));
            }
            catch (Exception ex)
            {
                return new ExecutionResult<T?>(false, ex.Message, default);
            }          
        }

        public async Task<ExecutionResult> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                var json = JsonSerializer.Serialize(value);
                var res = await _db.StringSetAsync(key, json, expiry);

                return new ExecutionResult(res, string.Empty);
            }
            catch (Exception ex)
            {
                return new ExecutionResult(false, ex.Message);
            }           
        }

        public async Task<ExecutionResult> RemoveAsync(string key)
        {
            try
            {
                var res = await _db.KeyDeleteAsync(key);

                return new ExecutionResult(res, "Couldn't find object in cache");
            }
            catch (Exception ex)
            {
                return new ExecutionResult(false, ex.Message);
            }
        }        
    }
}
