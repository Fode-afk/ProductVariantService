using StackExchange.Redis;
using System.Text.Json;

namespace ProductService.Data.Caching
{
    public class CacheRepo(IConnectionMultiplexer connectionMultiplexer) : ICacheRepo
    {
        private readonly IDatabase _db = connectionMultiplexer.GetDatabase();

        public async Task<ExecutionResult<List<T?>>> GetManyAsync<T>(string[] keys)
        {
            try
            {
                var values = await _db.StringGetAsync([.. keys.Select(k => (RedisKey)k)]);

                var result = new List<T?>();

                foreach (var value in values)
                {
                    if (value.HasValue)
                        result.Add(JsonSerializer.Deserialize<T>(value!));
                }

                if (result.Count == 0)
                    return new ExecutionResult<List<T?>>(false, string.Empty, null);

                return new ExecutionResult<List<T?>>(true, string.Empty, result);
            }
            catch (Exception ex)
            {
                return new ExecutionResult<List<T?>>(false, ex.Message, []);
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
