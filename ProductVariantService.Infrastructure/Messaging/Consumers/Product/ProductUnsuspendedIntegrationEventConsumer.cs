using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Products;
using ProductVariantService.Application.Features.IntegrationEventHandlers.ProductSnapshot.UpdateProductSnapshot;

namespace ProductVariantService.Infrastructure.Messaging.Consumers.Product;

public sealed class ProductUnsuspendedIntegrationEventConsumer(IMediator mediator) : IConsumer<ProductUnsuspendedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductUnsuspendedIntegrationEvent> context) =>
        await mediator.Send(new UpdateProductSnapshotCommand(
            context.Message.ProductId,
            context.Message.CategoryId,
            context.Message.CanEditContent,
            context.Message.Version), context.CancellationToken);
}
