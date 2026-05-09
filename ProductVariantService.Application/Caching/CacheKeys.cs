namespace ProductVariantService.Application.Caching;

public static class CacheKeys
{
    public static string ProductVariantsByProductId(Guid productId, string currency) =>
        $"ProductVariants:{productId}:{currency}";

    public static string ExchangeRateByCurrency(string currency) => $"ExchangeRate:{currency}";
}
