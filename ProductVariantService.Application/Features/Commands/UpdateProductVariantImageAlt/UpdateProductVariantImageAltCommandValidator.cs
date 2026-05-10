using FluentValidation;
using ProductVariantService.Domain.Errors;
using ProductVariantService.Domain.ValueObjects;

namespace ProductVariantService.Application.Features.Commands.UpdateProductVariantImageAlt;

public sealed class UpdateProductVariantImageAltCommandValidator : AbstractValidator<UpdateProductVariantImageAltCommand>
{
    public UpdateProductVariantImageAltCommandValidator()
    {
        RuleFor(x => x.VendorId)
           .NotEmpty().WithErrorCode(ProductVariantErrorCodes.InvalidId);

        RuleFor(x => x.ProductVariantId)
            .NotEmpty().WithErrorCode(ProductVariantErrorCodes.InvalidId);
  
        RuleFor(x => x.ImageId)
            .NotEmpty().WithErrorCode(ProductVariantErrorCodes.InvalidId);

        RuleFor(x => x.AltText)
            .NotEmpty().WithErrorCode(AltTextErrorCodes.NullOrEmpty)
            .MaximumLength(AltText.MaxLength).WithErrorCode(AltTextErrorCodes.TooLong);
    }
}
