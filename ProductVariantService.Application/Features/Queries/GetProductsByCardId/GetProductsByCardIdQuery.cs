using MediatR;
using migApp.Shared.Results;
using ProductVariantService.Application.Dtos;

namespace ProductVariantService.Application.Features.Queries.GetProductsByCardId;

public sealed record GetProductsByCardIdQuery(
    Guid ProductCardId,
    string Currency) : IRequest<IResult<IEnumerable<ProductDto>>>;