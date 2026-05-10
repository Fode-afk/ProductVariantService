using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.ProductVariants;
using ProductVariantService.Domain.DomainEvents;
using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductImageRemovedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductImageRemovedDomainEvent>
{
    public async Task Handle(ProductImageRemovedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductImageRemovedIntegrationEvent(
            notification.ProductVariantId,
            notification.HasMainImage,
            notification.Version), cancellationToken);
}
