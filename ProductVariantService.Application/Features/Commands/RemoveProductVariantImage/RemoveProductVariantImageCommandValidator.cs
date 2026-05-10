using FluentValidation;
using ProductVariantService.Domain.Errors;

namespace ProductVariantService.Application.Features.Commands.RemoveProductVariantImage;

public sealed class RemoveProductVariantImageCommandValidator : AbstractValidator<RemoveProductVariantImageCommand>
{
    public RemoveProductVariantImageCommandValidator()
    {
        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductVariantErrorCodes.InvalidId);

        RuleFor(x => x.ProductVariantId)
            .NotEmpty().WithErrorCode(ProductVariantErrorCodes.InvalidId);

        RuleFor(x => x.ImageId)
            .NotEmpty().WithErrorCode(ProductVariantErrorCodes.InvalidId);
    }
}
