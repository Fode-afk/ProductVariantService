using MediatR;
using migApp.Shared.Results;

namespace ProductVariantService.Application.Features.Commands.ReorderProductVariantImages;

public sealed record ReorderProductVariantImagesCommand(
    Guid ProductVariantId,
    Guid VendorId, 
    List<Guid> ImageIds) : IRequest<IResult>;