using FluentValidation;
using ProductVariantService.Domain.Errors;
using ProductVariantService.Domain.ValueObjects;

namespace ProductVariantService.Application.Features.Commands.AddProductVariantImage;

public sealed class AddProductVariantImageCommandValidator : AbstractValidator<AddProductVariantImageCommand>
{
    public AddProductVariantImageCommandValidator()
    {
        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductVariantErrorCodes.InvalidId);

        RuleFor(x => x.ProductVariantId)
            .NotEmpty().WithErrorCode(ProductVariantErrorCodes.InvalidId);

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithErrorCode(ImageUrlErrorCodes.NullOrEmpty)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            .WithErrorCode(ImageUrlErrorCodes.InvalidFormat);

        RuleFor(x => x.AltText)
            .NotEmpty().WithErrorCode(AltTextErrorCodes.NullOrEmpty)
            .MaximumLength(AltText.MaxLength).WithErrorCode(AltTextErrorCodes.TooLong);
    }
}
