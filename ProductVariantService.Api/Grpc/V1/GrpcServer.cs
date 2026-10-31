using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using migApp.Shared.Grpc;
using ProductVariantService.Api.Grpc.Mappers;
using ProductVariantService.Api.Grpc.V1.Protos;
using ProductVariantService.Application.Features.Commands.DeleteProductVariant;
using ProductVariantService.Application.Features.Commands.RemoveProductVariantImage;

namespace ProductVariantService.Api.Grpc.V1;

internal sealed class GrpcServer(ISender sender) : Protos.ProductVariantService.ProductVariantServiceBase
{
    public override async Task<ProductVariantWriteAck> CreateProductVariant(CreateProductVariantRequest request, ServerCallContext context) =>
        (await sender.Send(request.ToCommand(), context.CancellationToken))
            .ThrowIfFailure().ToResponse();
    
    public override async Task<ProductVariantWriteAck> UpdateProductVariantInfo(UpdateProductVariantInfoRequest request, ServerCallContext context) =>
        (await sender.Send(request.ToCommand(), context.CancellationToken))
            .ThrowIfFailure().ToResponse();

    public override async Task<ProductVariantWriteAck> AddProductVariantImage(AddProductVariantImageRequest request, ServerCallContext context) =>
         (await sender.Send(request.ToCommand(), context.CancellationToken))
            .ThrowIfFailure().ToResponse();

    public override async Task<ProductVariantWriteAck> RemoveProductVariantImage(RemoveProductVariantImageRequest request, ServerCallContext context) =>
        (await sender.Send(
            new RemoveProductVariantImageCommand(
                Guid.Parse(request.ProductVariantId),
                Guid.Parse(request.VendorId),
                Guid.Parse(request.ImageId)), context.CancellationToken)).ThrowIfFailure().ToResponse();

    public override async Task<ProductVariantWriteAck> ReorderProductVariantImages(ReorderProductVariantImagesRequest request, ServerCallContext context) =>
         (await sender.Send(request.ToCommand(), context.CancellationToken))
            .ThrowIfFailure().ToResponse();

    public override async Task<ProductVariantWriteAck> UpdateProductVariantImageAlt(UpdateProductVariantImageAltRequest request, ServerCallContext context) =>
         (await sender.Send(request.ToCommand(), context.CancellationToken))
            .ThrowIfFailure().ToResponse();

    public override async Task<Empty> DeleteProductVariant(DeleteProductVariantRequest request, ServerCallContext context)
    {
        var result = await sender.Send(
            new DeleteProductVariantCommand(
                Guid.Parse(request.ProductVariantId),
                Guid.Parse(request.VendorId)), context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }
}