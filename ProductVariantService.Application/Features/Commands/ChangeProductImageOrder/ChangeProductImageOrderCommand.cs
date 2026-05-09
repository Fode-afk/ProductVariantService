using MediatR;
using migApp.Shared.Results;

namespace ProductVariantService.Application.Features.Commands.ChangeProductImageOrder;

public sealed record ChangeProductImageOrderCommand(
    Guid ProductId,
    Guid VendorId,
    string Url,
    int NewOrder) : IRequest<IResult>;