using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Messaging.IntegrationEvents.Discounts;
using migApp.Shared.Messaging.IntegrationEvents.Products;
using migApp.Shared.Results;
using ProductVariantService.Application.Interfaces.Data;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.Commands.ValidatePorductForDiscount;

public sealed class ValidatePorductForDiscountCommandHandler(
    IAppDbContext context,
    IPublishEndpoint publish) : IRequestHandler<ValidatePorductForDiscountCommand, IResult>
{
    public async Task<IResult> Handle(ValidatePorductForDiscountCommand request, CancellationToken cancellationToken)
    {
        //var productExists = await context.Products
        //    .AnyAsync(p =>
        //        p.Id == request.ProductId &&
        //        p.ProductCardId == request.ProductCardId, 
        //        cancellationToken);
        //
        //if (productExists)
        //    await publish.Publish(new ProductValidated(request.CorrelationId), cancellationToken);
        //else
        //    await publish.Publish(new DiscountProcessFailed(
        //        request.CorrelationId,
        //        "Product not found or does not belong to vendor"), cancellationToken);

        return Ok();
    }
}