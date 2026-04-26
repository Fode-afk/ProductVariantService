using MediatR;
using migApp.Shared.Results;

namespace ProductService.Application.Features.Commands.UnmarkProductAsDefault;

public sealed record UnmarkProductAsDefaultCommand(
    Guid ProductId,
    Guid VendorId) : IRequest<IResult>;