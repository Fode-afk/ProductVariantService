using FluentValidation;
using ProductVariantService.Domain.Errors;

namespace ProductVariantService.Application.Features.Commands.ReorderProductVariantImages;

public sealed class ReorderProductVariantImagesCommandValidator : AbstractValidator<ReorderProductVariantImagesCommand>
{
    public ReorderProductVariantImagesCommandValidator()
    {
        RuleFor(x => x.VendorId)
           .NotEmpty().WithErrorCode(ProductVariantErrorCodes.InvalidId);

        RuleFor(x => x.ProductVariantId)
            .NotEmpty().WithErrorCode(ProductVariantErrorCodes.InvalidId);

        RuleForEach(x => x.ImageIds)
            .NotEmpty().WithErrorCode(ProductVariantErrorCodes.InvalidId);
    }
}
