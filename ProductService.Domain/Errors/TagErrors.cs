using migApp.Shared.Results;

namespace ProductService.Domain.Errors;

public static class TagErrors
{
    public static Error Empty() => Error.InvalidArgument(TagErrorCodes.Empty);
    public static Error TooLong() => Error.InvalidArgument(TagErrorCodes.TooLong);
}

public static class TagErrorCodes
{
    public const string Empty = "Tag.Empty";
    public const string TooLong = "Tag.TooLong";
}