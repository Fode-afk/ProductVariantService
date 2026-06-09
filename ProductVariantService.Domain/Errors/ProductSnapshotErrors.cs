using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class ProductSnapshotErrors
{
    public static Error NotFound() => 
        Error.NotFound(ProductSnapshotErrorCodes.NotFound,
            "Product snapshot not found.");

    public static Error CannotModify() =>
        Error.InvalidArgument(ProductSnapshotErrorCodes.CannotModify,
            "Cannot modify the product snapshot.");

    public static Error DoesNotBelongToVendor() =>
        Error.Unauthenticated(ProductSnapshotErrorCodes.DoesNotBelongToVendor,
            "The product snapshot does not belong to the authenticated vendor.");
}

public static class ProductSnapshotErrorCodes
{
    public const string NotFound = "ProductSnapshot.NotFound";
    public const string CannotModify = "ProductSnapshot.CannotModify";
    public const string DoesNotBelongToVendor = "ProductSnapshot.DoesNotBelongToVendor";
}