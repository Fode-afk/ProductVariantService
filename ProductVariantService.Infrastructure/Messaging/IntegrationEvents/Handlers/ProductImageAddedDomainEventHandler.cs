using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.ProductVariants;
using ProductVariantService.Domain.DomainEvents;
using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductImageAddedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductImageAddedDomainEvent>
{
    public async Task Handle(ProductImageAddedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductImageAddedIntegrationEvent(
            notification.ProductVariantId,
            notification.HasMainImage,
            notification.Version), cancellationToken);
}
