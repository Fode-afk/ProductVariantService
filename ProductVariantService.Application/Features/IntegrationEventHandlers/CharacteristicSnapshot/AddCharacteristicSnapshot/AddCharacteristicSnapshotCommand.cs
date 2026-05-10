using MediatR;
using migApp.Shared.Enums.Characteristics;
using migApp.Shared.Results;

namespace ProductVariantService.Application.Features.IntegrationEventHandlers.CharacteristicSnapshot.AddCharacteristicSnapshot;

public sealed record AddCharacteristicSnapshotCommand(
    Guid CharacteristicId,
    string Name,
    AttributeCharType CharType,
    string? GroupName,
    bool IsUnifying,
    Guid CategoryId,
    long Version) : IRequest<IResult>;