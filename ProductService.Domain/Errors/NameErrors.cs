using migApp.Shared.Results;

namespace ProductService.Domain.Errors;

public static class NameErrors
{
    public static Error NullOrEmpty() => Error.InvalidArgument(NameErrorCodes.NullOrEmpty);
    public static Error TooShort() => Error.InvalidArgument(NameErrorCodes.TooShort);
    public static Error TooLong() => Error.InvalidArgument(NameErrorCodes.TooLong);
}

public static class NameErrorCodes
{
    public const string NullOrEmpty = "Name.NullOrEmpty";
    public const string TooShort = "Name.TooShort";
    public const string TooLong = "Name.TooLong";

}