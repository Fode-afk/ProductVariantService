using migApp.Shared.Results;

namespace ProductService.Domain.Errors;

public static class ProductImageErrors
{
    public static Error InvalidSortOrder() => Error.InvalidArgument(ProductImageErrorCodes.InvalidSortOrder);
}

public static class ProductImageErrorCodes
{
    public const string InvalidSortOrder = "ProductImage.InvalidSortOrder";
}