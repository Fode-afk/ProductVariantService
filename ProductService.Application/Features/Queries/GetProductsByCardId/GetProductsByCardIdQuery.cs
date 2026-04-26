using MediatR;
using migApp.Shared.Results;
using ProductService.Application.Dtos;

namespace ProductService.Application.Features.Queries.GetProductsByCardId;

public sealed record GetProductsByCardIdQuery(Guid ProductCardId) : IRequest<IResult<IEnumerable<ProductDto>>>;