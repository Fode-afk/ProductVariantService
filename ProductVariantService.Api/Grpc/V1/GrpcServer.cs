using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using migApp.Shared.Grpc;
using ProductVariantService.Api.Grpc.Mappers;
using ProductVariantService.Api.Grpc.V1.Protos;
using ProductVariantService.Application.Features.Commands.DeleteProductVariant;
using ProductVariantService.Application.Features.Commands.RemoveProductVariantImage;

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
    
    public override async Task<Empty> UpdateProductVariantInfo(UpdateProductVariantInfoRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductVariantGrpcMapper.ToUpdateProductVariantInfoCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> AddProductVariantImage(AddProductVariantImageRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductVariantGrpcMapper.ToAddProductVariantImageCommand(request),
            context.CancellationToken); 
        result.ThrowIfFailure();
        return new Empty();
    }
    
    public override async Task<Empty> RemoveProductVariantImage(RemoveProductVariantImageRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            new RemoveProductVariantImageCommand(
                Guid.Parse(request.ProductVariantId),
                Guid.Parse(request.VendorId),
                Guid.Parse(request.ImageId)), context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> ReorderProductVariantImages(ReorderProductVariantImagesRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductVariantGrpcMapper.ToReorderProductVariantImagesCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> UpdateProductVariantImageAlt(UpdateProductVariantImageAltRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductVariantGrpcMapper.ToUpdateProductVariantImageAltCommand(request),
            context.CancellationToken); 
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> DeleteProductVariant(DeleteProductVariantRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            new DeleteProductVariantCommand(
                Guid.Parse(request.ProductVariantId),
                Guid.Parse(request.VendorId)), context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }
}