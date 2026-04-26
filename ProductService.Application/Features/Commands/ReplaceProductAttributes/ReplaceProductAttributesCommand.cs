using MediatR;
using migApp.Shared.Results;

namespace ProductService.Application.Features.Commands.ReplaceProductAttributes;

public sealed record ReplaceProductAttributesCommand(
    Guid ProductId,
    Guid VendorId,
    Dictionary<string, string> Attributes) : IRequest<IResult>;