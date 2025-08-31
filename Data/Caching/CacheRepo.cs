using StackExchange.Redis;
using System.Text.Json;

namespace ProductService.Data.Caching
{
    public class CacheRepo(IConnectionMultiplexer connectionMultiplexer) : ICacheRepo
    {
        private readonly IDatabase _db = connectionMultiplexer.GetDatabase();

        public async Task<ExecutionResult<List<T?>>> GetManyAsync<T>(string[] keys, TimeSpan? slidingExpiration = null)
        {
            try
            {
                var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
                var values = await _db.StringGetAsync(redisKeys);

                var result = new List<T?>();

                for (int i = 0; i < redisKeys.Length; i++)
                {
                    var value = values[i];
                    if (value.HasValue)
                    {
                        result.Add(JsonSerializer.Deserialize<T>(value!));

                        if (slidingExpiration != null)
                        {
                            await _db.KeyExpireAsync(redisKeys[i], slidingExpiration);
                        }
                    }
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

                if (res)
                    return new ExecutionResult(res, string.Empty);

                return new ExecutionResult(res, "Couldn't find object in cache");
            }
            catch (Exception ex)
            {
                return new ExecutionResult(false, ex.Message);
            }
        }        
    }
}
