using migApp.Shared.Results;

namespace ProductService.Domain.Errors;

public static class BarcodeErrors
{
    public static Error NullOrEmpty() => Error.InvalidArgument(BarcodeErrorCodes.NullOrEmpty);
    public static Error InvalidFormat() => Error.InvalidArgument(BarcodeErrorCodes.InvalidFormat);
}

public static class BarcodeErrorCodes
{
    public const string NullOrEmpty = "Barcode.NullOrEmpty";
    public const string InvalidFormat = "Barcode.InvalidFormat";
}