using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class SkuErrors
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(SkuErrorCodes.NullOrEmpty,
            "SKU cannot be null or empty.");

    public static Error InvalidFormat() =>
        Error.InvalidArgument(SkuErrorCodes.InvalidFormat,
            "Invalid SKU format.");
}

public static class SkuErrorCodes
{
    public const string NullOrEmpty = "Sku.NullOrEmpty";
    public const string InvalidFormat = "Sku.InvalidFormat";
}
