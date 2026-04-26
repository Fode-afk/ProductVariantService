using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using migApp.Shared.Grpc;
using ProductService.Api.Grpc.Mappers;
using ProductService.Api.Grpc.V1.Protos;
using ProductService.Application.Features.Queries.GetProductsByCardId;

namespace ProductService.Api.Grpc.V1;

internal sealed class GrpcServer(IMediator mediator) :  Protos.ProductService.ProductServiceBase
{
    public override async Task<Empty> CreateProduct(CreateProductRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductGrpcMapper.ToCreateProductCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<GetProductsByCardIdResponse> GetProductsByCardId(GetProductsByCardIdRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            new GetProductsByCardIdQuery(Guid.Parse(request.ProductCardId)),
            context.CancellationToken);
        var productDtos = result.ThrowIfFailure();

        return new GetProductsByCardIdResponse { Products = { productDtos.Select(ProductGrpcMapper.FromDto) } };
    }

    public override async Task<Empty> UpdateProductInfo(UpdateProductInfoRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductGrpcMapper.ToUpdateProductInfoCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> MarkProductAsDefault(MarkProductAsDefaultRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductGrpcMapper.ToMarkProductAsDefaultCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> UnmarkProductAsDefault(UnmarkProductAsDefaultRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductGrpcMapper.ToUnmarkProductAsDefaultCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> ReplaceProductAttributes(ReplaceProductAttributesRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductGrpcMapper.ToReplaceProductAttributesCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> ReplaceProductTags(ReplaceProductTagsRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductGrpcMapper.ToReplaceProductTagsCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> AddProductImage(AddProductImageRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductGrpcMapper.ToAddProductImageCommand(request),
            context.CancellationToken); 
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> RemoveProductImage(RemoveProductImageRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductGrpcMapper.ToRemoveProductImageCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> ChangeProductImageOrder(ChangeProductImageOrderRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductGrpcMapper.ToChangeProductImageOrderCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> SetMainProductImage(SetMainProductImageRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductGrpcMapper.ToSetMainProductImageCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> UpdateProductImageAlt(UpdateProductImageAltRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductGrpcMapper.ToUpdateProductImageAltCommand(request),
            context.CancellationToken); 
        result.ThrowIfFailure();
        return new Empty();
    }
}
