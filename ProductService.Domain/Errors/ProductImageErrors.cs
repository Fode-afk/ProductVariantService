using migApp.Shared.Results;

namespace ProductService.Domain.Errors;

public static class ProductImageErrors
{
    public static Error InvalidSortOrder() => Error.InvalidArgument(ProductImageErrorCodes.InvalidSortOrder);
    public static Error MaxImagesReached() => Error.InvalidArgument(ProductImageErrorCodes.MaxImagesReached);
}

public static class ProductImageErrorCodes
{
    public const string InvalidSortOrder = "ProductImage.InvalidSortOrder";
    public const string MaxImagesReached = "ProductImage.MaxImagesReached";
}