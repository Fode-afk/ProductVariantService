namespace ProductService.Data.Caching
{
    public interface ICacheRepo
    {
        Task<ExecutionResult<T?>> GetAsync<T>(string key);
        Task<ExecutionResult> SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<ExecutionResult> RemoveAsync(string key);
    }
}
