using MassTransit;
using migApp.Shared.Dtos.ProductVariant;
using migApp.Shared.Messaging.IntegrationEvents.ProductVariants;
using ProductVariantService.Domain.Contexts;
using ProductVariantService.Domain.Primitives;

namespace ProductVariantService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductVariantImagesReorderedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductVariantImagesReorderedDomainEvent>
{
    public async Task Handle(ProductVariantImagesReorderedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductVariantImagesReorderedIntegrationEvent(
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
