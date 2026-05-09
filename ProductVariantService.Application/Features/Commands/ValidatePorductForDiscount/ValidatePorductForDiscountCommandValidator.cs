using FluentValidation;

namespace ProductVariantService.Application.Features.Commands.ValidatePorductForDiscount;

public sealed class ValidatePorductForDiscountCommandValidator : AbstractValidator<ValidatePorductForDiscountCommand>
{
    public ValidatePorductForDiscountCommandValidator()
    {

    }
}
