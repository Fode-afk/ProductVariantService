using FluentValidation;
using ProductVariantService.Domain.Errors;

namespace ProductVariantService.Application.Features.Commands.UpdateProductVariantInfo;

public sealed class UpdateProductVariantInfoCommandValidator : AbstractValidator<UpdateProductVariantInfoCommand>
{
    public UpdateProductVariantInfoCommandValidator()
    {
        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductVariantErrorCodes.InvalidId);

        RuleFor(x => x.ProductVariantId)
            .NotEmpty().WithErrorCode(ProductVariantErrorCodes.InvalidId);

        RuleFor(x => x.Barcode)
            .NotEmpty().WithErrorCode(BarcodeErrorCodes.NullOrEmpty);

        RuleFor(x => x.SKU)
            .NotEmpty().WithErrorCode(SkuErrorCodes.NullOrEmpty);

        RuleFor(x => x.Dimensions)
            .NotNull().WithErrorCode(DimensionsErrorCodes.Invalid);

        When(x => x.Dimensions is not null, () =>
        {
            RuleFor(x => x.Dimensions.Length)
                .GreaterThan(0)
                .WithErrorCode(DimensionsErrorCodes.Invalid);

            RuleFor(x => x.Dimensions.Width)
                .GreaterThan(0)
                .WithErrorCode(DimensionsErrorCodes.Invalid);

            RuleFor(x => x.Dimensions.Height)
                .GreaterThan(0)
                .WithErrorCode(DimensionsErrorCodes.Invalid);
        });

        RuleFor(x => x.Weight)
            .NotNull().WithErrorCode(WeightErrorCodes.Invalid);

        When(x => x.Weight is not null, () =>
        {
            RuleFor(x => x.Weight.Value)
                .GreaterThan(0)
                .WithErrorCode(DimensionsErrorCodes.Invalid);
        });
    }
}