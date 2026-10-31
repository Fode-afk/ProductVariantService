using MassTransit;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Dtos.ProductVariant;
using migApp.Shared.Messaging.IntegrationEvents.ProductVariants;
using ProductVariantService.Domain.DomainEvents;

namespace ProductVariantService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductVariantImageRemovedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductVariantImageRemovedDomainEvent>
{
    public async Task Handle(ProductVariantImageRemovedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductVariantImageRemovedIntegrationEvent(
            notification.ProductVariantId,
            notification.ProductId,
            [.. notification.Images.Select(i =>
               new ProductVariantImageDto(
                   i.Url,
                   i.Alt,
                   i.SortOrder,
                   i.IsMain))],
            notification.HasMainImage,
            notification.Version), cancellationToken);
}
