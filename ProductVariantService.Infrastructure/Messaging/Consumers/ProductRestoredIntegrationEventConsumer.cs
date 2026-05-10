using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Products;
using ProductVariantService.Application.Features.IntegrationEventHandlers.ProductSnapshot.UpdateProductSnapshot;

namespace ProductVariantService.Infrastructure.Messaging.Consumers;

public sealed class ProductRestoredIntegrationEventConsumer(IMediator mediator) : IConsumer<ProductRestoredIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductRestoredIntegrationEvent> context) =>
        await mediator.Send(new UpdateProductSnapshotCommand(
            context.Message.ProductId,
            context.Message.CategoryId,
            context.Message.CanBeModified,
            context.Message.Version), context.CancellationToken);
}
