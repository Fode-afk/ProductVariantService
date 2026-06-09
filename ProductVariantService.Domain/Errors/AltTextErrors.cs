using migApp.Shared.Results;

namespace ProductVariantService.Domain.Errors;

public static class AltTextErrors
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(AltTextErrorCodes.NullOrEmpty,
            "Alt text cannot be null or empty.");

    public static Error TooLong(int maxLength) =>
        Error.InvalidArgument(AltTextErrorCodes.TooLong,
            $"Alt text is too long. Maximum length is {maxLength} characters.");
}

public static class AltTextErrorCodes
{
    public const string NullOrEmpty = "AltText.NullOrEmpty";
    public const string TooLong = "AltText.TooLong";
}