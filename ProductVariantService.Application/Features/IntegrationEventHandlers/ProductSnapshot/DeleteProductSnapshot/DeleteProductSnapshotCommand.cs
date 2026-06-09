using MediatR;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.ProductSnapshot.DeleteProductSnapshot;

public sealed record DeleteProductSnapshotCommand(Guid ProductId) : IRequest;