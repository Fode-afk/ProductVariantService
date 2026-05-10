using MediatR;
using migApp.Shared.Results;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.CharacteristicSnapshot.DeleteCharacteristicSnapshot;

public sealed record DeleteCharacteristicSnapshotCommand(Guid CharacteristicId) : IRequest<IResult>;