using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductVariantService.Application.Interfaces.Data;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.CharacteristicSnapshot.AddCharacteristicSnapshot;

public sealed class AddCharacteristicSnapshotCommandHandler(IAppDbContext context) : IRequestHandler<AddCharacteristicSnapshotCommand>
{
    public async Task Handle(AddCharacteristicSnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.CharacteristicSnapshots
            .AnyAsync(x => x.CharacteristicId == request.CharacteristicId, cancellationToken);
        if (exists)
            return;

        context.CharacteristicSnapshots.Add(
            new Domain.Snapshots.CharacteristicSnapshot
            {
                CharacteristicId = request.CharacteristicId,
                Name = request.Name,
                CharType = request.CharType,
                GroupName = request.GroupName,
                IsUnifying = request.IsUnifying,
                CategoryId = request.CategoryId
            });

        await context.SaveChangesAsync(cancellationToken);
    }
}