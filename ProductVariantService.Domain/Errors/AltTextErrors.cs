using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class AltTextErrors
{
    public static Error NullOrEmpty() => Error.InvalidArgument(AltTextErrorCodes.NullOrEmpty);
    public static Error TooLong() => Error.InvalidArgument(AltTextErrorCodes.TooLong);
}

public static class AltTextErrorCodes
{
    public const string NullOrEmpty = "AltText.NullOrEmpty";
    public const string TooLong = "AltText.TooLong";
}