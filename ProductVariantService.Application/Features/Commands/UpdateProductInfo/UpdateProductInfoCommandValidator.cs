using FluentValidation;
using ProductService.Domain.ValueObjects;

namespace ProductVariantService.Application.Features.Commands.UpdateProductInfo;

public sealed class UpdateProductInfoCommandValidator : AbstractValidator<UpdateProductInfoCommand>
{
    //public UpdateProductInfoCommandValidator()
    //{
    //    RuleFor(x => x.VendorId)
    //          .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);
    //
    //    RuleFor(x => x.Name)
    //        .NotEmpty().WithErrorCode(NameErrorCodes.NullOrEmpty)
    //        .MaximumLength(Name.MaxLength).WithErrorCode(NameErrorCodes.TooLong);
    //
    //    RuleFor(x => x.Dimensions)
    //        .NotNull().WithErrorCode(DimensionsErrorCodes.Invalid);
    //
    //    When(x => x.Dimensions is not null, () =>
    //    {
    //        RuleFor(x => x.Dimensions.Length)
    //            .GreaterThan(0)
    //            .WithErrorCode(DimensionsErrorCodes.Invalid);
    //
    //        RuleFor(x => x.Dimensions.Width)
    //            .GreaterThan(0)
    //            .WithErrorCode(DimensionsErrorCodes.Invalid);
    //
    //        RuleFor(x => x.Dimensions.Height)
    //            .GreaterThan(0)
    //            .WithErrorCode(DimensionsErrorCodes.Invalid);
    //    });
    //
    //    RuleFor(x => x.Weight)
    //        .NotNull().WithErrorCode(WeightErrorCodes.Invalid);
    //
    //    When(x => x.Weight is not null, () =>
    //    {
    //        RuleFor(x => x.Weight.Value)
    //            .GreaterThan(0)
    //            .WithErrorCode(DimensionsErrorCodes.Invalid);
    //    });
    //}
}