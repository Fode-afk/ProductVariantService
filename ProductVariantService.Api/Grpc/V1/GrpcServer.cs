using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using migApp.Shared.Grpc;
using ProductVariantService.Api.Grpc.Mappers;
using ProductVariantService.Api.Grpc.V1.Protos;

namespace ProductVariantService.Api.Grpc.V1;

internal sealed class GrpcServer(IMediator mediator) : Protos.ProductVariantService.ProductVariantServiceBase
{
    public override async Task<Empty> CreateProductVariant(CreateProductVariantRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductVariantGrpcMapper.ToCreateProductVariantCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    //public override async Task<GetProductsByCardIdResponse> GetProductsByCardId(GetProductsByCardIdRequest /request, /ServerCallContext context)
    //{
    //    var result = await mediator.Send(
    //        new GetProductsByCardIdQuery(Guid.Parse(request.ProductCardId), request.CurrencyCode),
    //        context.CancellationToken);
    //    var productDtos = result.ThrowIfFailure();
    //
    //    return new GetProductsByCardIdResponse { Products = { productDtos.Select//(ProductVariantGrpcMapper.FromDto) } };
    //}
    //
    //public override async Task<Empty> UpdateProductInfo(UpdateProductInfoRequest request, ServerCallContext /context)
    //{
    //    var result = await mediator.Send(
    //        ProductVariantGrpcMapper.ToUpdateProductInfoCommand(request),
    //        context.CancellationToken);
    //    result.ThrowIfFailure();
    //    return new Empty();
    //}
    //
    //public override async Task<Empty> AddProductImage(AddProductImageRequest request, ServerCallContext context)
    //{
    //    var result = await mediator.Send(
    //        ProductVariantGrpcMapper.ToAddProductImageCommand(request),
    //        context.CancellationToken); 
    //    result.ThrowIfFailure();
    //    return new Empty();
    //}
    //
    //public override async Task<Empty> RemoveProductImage(RemoveProductImageRequest request, ServerCallContext //context)
    //{
    //    var result = await mediator.Send(
    //        ProductVariantGrpcMapper.ToRemoveProductImageCommand(request),
    //        context.CancellationToken);
    //    result.ThrowIfFailure();
    //    return new Empty();
    //}
    //
    //public override async Task<Empty> ChangeProductImageOrder(ChangeProductImageOrderRequest request, //ServerCallContext context)
    //{
    //    var result = await mediator.Send(
    //        ProductVariantGrpcMapper.ToChangeProductImageOrderCommand(request),
    //        context.CancellationToken);
    //    result.ThrowIfFailure();
    //    return new Empty();
    //}
    //
    //public override async Task<Empty> SetMainProductImage(SetMainProductImageRequest request, ServerCallContext //context)
    //{
    //    var result = await mediator.Send(
    //        ProductVariantGrpcMapper.ToSetMainProductImageCommand(request),
    //        context.CancellationToken);
    //    result.ThrowIfFailure();
    //    return new Empty();
    //}
    //
    //public override async Task<Empty> UpdateProductImageAlt(UpdateProductImageAltRequest request, /ServerCallContext /context)
    //{
    //    var result = await mediator.Send(
    //        ProductVariantGrpcMapper.ToUpdateProductImageAltCommand(request),
    //        context.CancellationToken); 
    //    result.ThrowIfFailure();
    //    return new Empty();
    //}
}
