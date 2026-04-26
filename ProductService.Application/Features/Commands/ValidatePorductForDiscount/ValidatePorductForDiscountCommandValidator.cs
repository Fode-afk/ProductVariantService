using FluentValidation;

namespace ProductService.Application.Features.Commands.ValidatePorductForDiscount;

public sealed class ValidatePorductForDiscountCommandValidator : AbstractValidator<ValidatePorductForDiscountCommand>
{
    public ValidatePorductForDiscountCommandValidator()
    {

    }
}
