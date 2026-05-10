using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductVariantService.Application.Interfaces.Data;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.CharacteristicSnapshot.AddCharacteristicSnapshot;

public sealed class AddCharacteristicSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<AddCharacteristicSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(AddCharacteristicSnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.CharacteristicSnapshots
            .AnyAsync(x => x.CharacteristicId == request.CharacteristicId, cancellationToken);
        if (exists)
            return Ok();

        context.CharacteristicSnapshots.Add(
            new Domain.Snapshots.CharacteristicSnapshot
            {
                CharacteristicId = request.CharacteristicId,
                Name = request.Name,
                CharType = request.CharType,
                GroupName = request.GroupName,
                IsUnifying = request.IsUnifying,
                CategoryId = request.CategoryId,
                UpdatedAt = timeProvider.GetUtcNow(),
                Version = request.Version
            });

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}