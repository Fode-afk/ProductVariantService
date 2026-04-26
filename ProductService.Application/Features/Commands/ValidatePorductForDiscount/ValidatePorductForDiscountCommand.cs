using MediatR;
using migApp.Shared.Results;

namespace ProductService.Application.Features.Commands.ValidatePorductForDiscount;

public sealed record ValidatePorductForDiscountCommand(
    Guid CorrelationId,
    Guid VendorId,
    Guid ProductCardId,
    Guid ProductId) : IRequest<IResult>;