using MediatR;
using migApp.Shared.Results;
using ProductVariantService.Application.Dtos;

namespace ProductVariantService.Application.Features.Commands.ReorderProductVariantImages;

public sealed record ReorderProductVariantImagesCommand(
    Guid ProductVariantId,
    Guid VendorId, 
    List<Guid> ImageIds) : IRequest<IResult<ProductVariantWriteAck>>;