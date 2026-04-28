using MediatR;
using migApp.Shared.Results;
using ProductService.Application.Dtos;

namespace ProductService.Application.Features.Queries.GetProductsByCardId;

public sealed record GetProductsByCardIdQuery(
    Guid ProductCardId,
    string Currency) : IRequest<IResult<IEnumerable<ProductDto>>>;