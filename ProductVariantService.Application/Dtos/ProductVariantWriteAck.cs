namespace ProductVariantService.Application.Dtos;

public sealed record ProductVariantWriteAck(Guid ProductVariantId, long Version);