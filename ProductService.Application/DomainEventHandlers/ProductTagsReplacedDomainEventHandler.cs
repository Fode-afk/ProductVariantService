using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces.Data;
using ProductService.Domain.DomainEvents;
using ProductService.Domain.Primitives;
using System.Text.Json;

namespace ProductService.Application.DomainEventHandlers;

public sealed class ProductTagsReplacedDomainEventHandler(IAppDbContext context) : IPreCommitDomainEventHandler<ProductTagsReplacedDomainEvent>
{
    public async Task Handle(ProductTagsReplacedDomainEvent notification, CancellationToken cancellationToken)
    {
        var productReadModel = await context.ProductReadModels
           .FirstOrDefaultAsync(x => x.Id == notification.ProductId, cancellationToken);

        if (productReadModel == null)
            return;

        var tags = notification.Tags
            .Select(t => t.Value)
            .ToList();

        productReadModel.TagsJson = JsonSerializer.Serialize(tags);
        productReadModel.TagsFlat = string.Join(",", tags);
        productReadModel.UpdatedAt = notification.UpdatedAt;
    }
}
