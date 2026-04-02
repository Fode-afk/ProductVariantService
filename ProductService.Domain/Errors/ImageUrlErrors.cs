using migApp.Shared.Results;

namespace ProductService.Domain.Errors;

public static class ImageUrlErrors
{
    public static Error NullOrEmpty() => Error.InvalidArgument(ImageUrlErrorCodes.NullOrEmpty);
    public static Error InvalidFormat() => Error.InvalidArgument(ImageUrlErrorCodes.InvalidFormat);
}

public static class ImageUrlErrorCodes
{
    public const string NullOrEmpty = "ImageUrl.NullOrEmpty";
    public const string InvalidFormat = "ImageUrl.InvalidFormat";
}
