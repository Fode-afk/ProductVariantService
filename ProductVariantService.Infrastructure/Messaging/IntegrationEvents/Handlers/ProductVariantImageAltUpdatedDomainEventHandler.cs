using MassTransit;
using migApp.Shared.Dtos.ProductVariant;
using migApp.Shared.Messaging.IntegrationEvents.ProductVariants;
using ProductVariantService.Domain.DomainEvents;
using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductVariantImageAltUpdatedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductVariantImageAltUpdatedDomainEvent>
{
    public async Task Handle(ProductVariantImageAltUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductVariantImageAltUpdatedIntegrationEvent(
            notification.ProductVariantId,
            notification.ProductId,
            [.. notification.Images.Select(i =>
               new ProductVariantImageDto(
                   i.Url,
                   i.Alt,
                   i.SortOrder,
                   i.IsMain))],
            notification.Version), cancellationToken);
}
