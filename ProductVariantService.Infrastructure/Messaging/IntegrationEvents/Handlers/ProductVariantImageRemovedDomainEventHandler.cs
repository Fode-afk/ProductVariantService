using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.ProductVariants;
using ProductVariantService.Domain.DomainEvents;
using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductVariantImageRemovedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductVariantImageRemovedDomainEvent>
{
    public async Task Handle(ProductVariantImageRemovedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductVariantImageRemovedIntegrationEvent(
            notification.ProductVariantId,
            notification.HasMainImage,
            notification.Version), cancellationToken);
}
