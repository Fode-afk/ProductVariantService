using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class ProductVariantImageErrors
{
    public static Error InvalidSortOrder() => Error.InvalidArgument(ProductVariantImageErrorCodes.InvalidSortOrder);
    public static Error MaxImagesReached() => Error.InvalidArgument(ProductVariantImageErrorCodes.MaxImagesReached);
    public static Error AlreadyExists() => Error.AlreadyExists(ProductVariantImageErrorCodes.AlreadyExists);
    public static Error NotFound() => Error.NotFound(ProductVariantImageErrorCodes.NotFound);
    public static Error InvalidImageCount() => Error.InvalidArgument(ProductVariantImageErrorCodes.InvalidImageCount);
    public static Error DuplicateImageIds() => Error.InvalidArgument(ProductVariantImageErrorCodes.DuplicateImageIds);
}

public static class ProductVariantImageErrorCodes
{
    public const string InvalidSortOrder = "ProductVariantImage.InvalidSortOrder";
    public const string MaxImagesReached = "ProductVariantImage.MaxImagesReached";
    public const string AlreadyExists = "ProductVariantImage.AlreadyExists";
    public const string NotFound = "ProductVariantImage.NotFound";
    public const string InvalidImageCount = "ProductVariantImage.InvalidImageCount";
    public const string DuplicateImageIds = "ProductVariantImage.DuplicateImageIds";
}