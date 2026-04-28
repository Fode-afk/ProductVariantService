namespace ProductService.Application.Caching;

public static class CacheTags
{
    public static string ProductsByCardId(Guid productCardId) => $"Products:{productCardId}";
}