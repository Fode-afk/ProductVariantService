using MediatR;
using migApp.Shared.Results;

namespace ProductVariantService.Application.Features.Commands.DeleteProductVariant;

public sealed record DeleteProductVariantCommand(
    Guid ProductVariantId,
    Guid VendorId) : IRequest<IResult>;