using MassTransit;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Messaging.IntegrationEvents.ProductVariants;
using ProductVariantService.Domain.DomainEvents;

namespace ProductVariantService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductVariantForceDeletedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductVariantForceDeletedDomainEvent>
{
    public async Task Handle(ProductVariantForceDeletedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductVariantForceDeletedIntegrationEvent(
            notification.ProductVariantId,
            notification.ProductId), cancellationToken);
}