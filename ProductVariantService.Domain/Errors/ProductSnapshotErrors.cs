using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class ProductSnapshotErrors
{
    public static Error NotFound() => Error.NotFound(ProductSnapshotErrorCodes.NotFound);
    public static Error InvalidVendor() => Error.Unauthenticated(ProductSnapshotErrorCodes.InvalidVendor);
    public static Error AlreadyExists() => Error.AlreadyExists(ProductSnapshotErrorCodes.AlreadyExists);
    public static Error CannotModify() => Error.InvalidArgument(ProductSnapshotErrorCodes.CannotModify);
    public static Error CannotModifyWhenPublished() => Error.InvalidArgument(ProductSnapshotErrorCodes.CannotModifyWhenPublished);
    public static Error CannotModifyWhenArchived() => Error.InvalidArgument(ProductSnapshotErrorCodes.CannotModifyWhenArchived);
    public static Error DoesNotBelongToVendor() => Error.Unauthenticated(ProductSnapshotErrorCodes.DoesNotBelongToVendor);
}

public static class ProductSnapshotErrorCodes
{
    public const string NotFound = "ProductSnapshot.NotFound";
    public const string InvalidVendor = "ProductSnapshot.InvalidVendor";
    public const string AlreadyExists = "ProductSnapshot.AlreadyExists";
    public const string CannotModify = "ProductSnapshot.CannotModify";
    public const string CannotModifyWhenPublished = "ProductSnapshot.CannotModifyWhenPublished";
    public const string CannotModifyWhenArchived = "ProductSnapshot.CannotModifyWhenArchived";
    public const string DoesNotBelongToVendor = "ProductSnapshot.DoesNotBelongToVendor";
}