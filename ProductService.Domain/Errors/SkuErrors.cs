using migApp.Shared.Results;

namespace ProductService.Domain.Errors;

public static class SkuErrors
{
    public static Error NullOrEmpty() => Error.InvalidArgument(SkuErrorCodes.NullOrEmpty);
    public static Error InvalidFormat() => Error.InvalidArgument(SkuErrorCodes.InvalidFormat);
}

public static class SkuErrorCodes
{
    public const string NullOrEmpty = "Sku.NullOrEmpty";
    public const string InvalidFormat = "Sku.InvalidFormat";
}
