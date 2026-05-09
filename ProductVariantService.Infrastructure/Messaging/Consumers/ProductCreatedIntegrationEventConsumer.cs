using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Products;
using ProductVariantService.Application.Features.Commands.AddProductSnapshot;

namespace ProductVariantService.Infrastructure.Messaging.Consumers;

public sealed class ProductCreatedIntegrationEventConsumer(IMediator mediator) : IConsumer<ProductCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductCreatedIntegrationEvent> context) => 
        await mediator.Send(new AddProductSnapshotCommand(
            context.Message.ProductId,
            context.Message.VendorId,
            context.Message.CategoryId,
            context.Message.CanBeModified), context.CancellationToken);
}