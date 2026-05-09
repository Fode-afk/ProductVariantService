using FluentValidation;
using ProductVariantService.Domain.Errors;

namespace ProductVariantService.Application.Features.Commands.AddProductSnapshot;

public sealed class AddProductSnapshotCommandValidator : AbstractValidator<AddProductSnapshotCommand>
{
    public AddProductSnapshotCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithErrorCode(ProductVariantErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductVariantErrorCodes.InvalidId);

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithErrorCode(ProductVariantErrorCodes.InvalidId);
    }
}