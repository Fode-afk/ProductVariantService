using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class BarcodeErrors
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(BarcodeErrorCodes.NullOrEmpty,
            "Barcode cannot be null or empty.");

    public static Error InvalidFormat() =>
        Error.InvalidArgument(BarcodeErrorCodes.InvalidFormat,
            "Barcode format is invalid.");
}

public static class BarcodeErrorCodes
{
    public const string NullOrEmpty = "Barcode.NullOrEmpty";
    public const string InvalidFormat = "Barcode.InvalidFormat";
}