using MediatR;
using migApp.Shared.Results;

namespace ProductService.Application.Features.Commands.MarkProductAsDefault;

public sealed record MarkProductAsDefaultCommand(
    Guid ProductId,
    Guid VendorId) : IRequest<IResult>;