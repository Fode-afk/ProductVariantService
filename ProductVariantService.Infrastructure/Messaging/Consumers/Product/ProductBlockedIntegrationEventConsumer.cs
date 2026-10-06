using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Products;
using ProductVariantService.Application.Features.IntegrationEventHandlers.ProductSnapshot.UpdateProductSnapshot;

namespace ProductVariantService.Infrastructure.Messaging.Consumers.Product;

public sealed class ProductBlockedIntegrationEventConsumer(IMediator mediator) : IConsumer<ProductBlockedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductBlockedIntegrationEvent> context) =>
        await mediator.Send(new UpdateProductSnapshotCommand(
            context.Message.ProductId,
            context.Message.CategoryId,
            context.Message.CanEditContent,
            context.Message.Version), context.CancellationToken);
}
