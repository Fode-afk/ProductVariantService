namespace ProductService.Application.Caching;

public static class CacheKeys
{
    public static string ProductsByCardId(Guid productCardId, string currency) =>
        $"Products:{productCardId}:{currency}";

    public static string ExchangeRateByCurrency(string currency) => $"ExchangeRate:{currency}";
}
