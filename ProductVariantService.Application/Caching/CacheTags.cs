namespace ProductVariantService.Application.Caching;

public static class CacheTags
{
    public static string ProductVariantsByProductId(Guid productId) => $"ProductVariant:{productId}";
}