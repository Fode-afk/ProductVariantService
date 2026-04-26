namespace ProductService.Application.Caching;

internal static class CacheKeys
{
    public static string ProductsByCardId(Guid productCardId) => $"Products:{productCardId}";
}
