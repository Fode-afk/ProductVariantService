using FluentValidation;

namespace ProductService.Application.Features.Commands.UpdateProductInfo;

public sealed class UpdateProductInfoCommandValidator : AbstractValidator<UpdateProductInfoCommand>
{
    public UpdateProductInfoCommandValidator()
    {
      
    }
}