using ProductVariantService.Api.Grpc.V1.Protos;
using ProductVariantService.Application.Features.Commands.AddProductVariantImage;
using ProductVariantService.Application.Features.Commands.CreateProductVariant;
using ProductVariantService.Application.Features.Commands.ReorderProductVariantImages;
using ProductVariantService.Application.Features.Commands.UpdateProductVariantImageAlt;
using ProductVariantService.Application.Features.Commands.UpdateProductVariantInfo;

namespace ProductVariantService.Api.Grpc.Mappers;

internal static class ProductVariantGrpcMapper
{
    public static CreateProductVariantCommand ToCommand(this CreateProductVariantRequest request) =>
        new(
            Guid.Parse(request.VendorId),
            Guid.Parse(request.ProductId),
            request.Dimensions.ToDto(),
            request.Weight.ToDto(),
            request.Barcode,
            request.Sku,
            request.Attributes.ToDictionary(a => Guid.Parse(a.Key), a => a.Value));

    public static UpdateProductVariantInfoCommand ToCommand(this UpdateProductVariantInfoRequest request) =>
        new(
            Guid.Parse(request.VendorId),
            Guid.Parse(request.ProductVariantId),
            request.Sku,
            request.Barcode,
            request.Dimensions.ToDto(),
            request.Weight.ToDto());
    
    public static AddProductVariantImageCommand ToCommand(this AddProductVariantImageRequest request) =>
        new(
            Guid.Parse(request.ProductVariantId),
            Guid.Parse(request.VendorId),
            request.Url,
            request.Alt);
    
    public static ReorderProductVariantImagesCommand ToCommand(this ReorderProductVariantImagesRequest request) =>
        new(
            Guid.Parse(request.ProductVariantId),
            Guid.Parse(request.VendorId),
            [.. request.OrderedImageIds.Select(Guid.Parse)]);

    public static UpdateProductVariantImageAltCommand ToCommand(this UpdateProductVariantImageAltRequest request) =>
        new(
            Guid.Parse(request.ProductVariantId),
            Guid.Parse(request.VendorId),
            Guid.Parse(request.ImageId),
            request.NewAlt);

    private static Application.Dtos.DimensionsDto ToDto(this DimensionsDto dto) =>
        new(
            dto.Length,
            dto.Width,
            dto.Height,
            dto.Unit);

    private static Application.Dtos.WeightDto ToDto(this WeightDto dto) =>
        new(
            dto.Value,
            dto.Unit);

    public static ProductVariantWriteAck ToResponse(this Application.Dtos.ProductVariantWriteAck ack) =>
        new()
        {
            ProductVariantId = ack.ProductVariantId.ToString(),
            Version = ack.Version
        };
}