using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductVariantService.Application.Interfaces.Data;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.CharacteristicSnapshot.DeleteCharacteristicSnapshot;

public sealed class DeleteCharacteristicSnapshotCommandHandler(IAppDbContext context) : IRequestHandler<DeleteCharacteristicSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(DeleteCharacteristicSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.CharacteristicSnapshots
            .FirstOrDefaultAsync(c => c.CharacteristicId == request.CharacteristicId, cancellationToken);
        if (snapshot is null)
            return Ok();

        context.CharacteristicSnapshots.Remove(snapshot);

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
