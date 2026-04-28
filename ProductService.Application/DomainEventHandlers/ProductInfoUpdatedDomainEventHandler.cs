using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces.Data;
using ProductService.Domain.DomainEvents;
using ProductService.Domain.Primitives;
using ProductService.Domain.ValueObjects;

namespace ProductService.Application.DomainEventHandlers;

public sealed class ProductInfoUpdatedDomainEventHandler(IAppDbContext context) : IPreCommitDomainEventHandler<ProductInfoUpdatedDomainEvent>
{
    public async Task Handle(ProductInfoUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var productReadModel = await context.ProductReadModels
            .FirstOrDefaultAsync(x => x.Id == notification.ProductId, cancellationToken);

        if (productReadModel == null)
            return;

        productReadModel.Name = notification.Name.Value;
        productReadModel.NameNormalized = Name.Normalize(notification.Name);

        var dimensions = notification.Dimensions;

        productReadModel.Length = dimensions.Length;
        productReadModel.Height = dimensions.Height;
        productReadModel.Width = dimensions.Width;
        productReadModel.DimensionUnit = dimensions.Unit.Code;

        var weight = notification.Weight;

        productReadModel.Weight = weight.Value;
        productReadModel.WeightUnit = weight.Unit.Code;

        productReadModel.UpdatedAt = notification.UpdatedAt;
    }
}
