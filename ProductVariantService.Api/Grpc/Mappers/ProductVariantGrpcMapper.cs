using ProductVariantService.Api.Grpc.V1.Protos;
using ProductVariantService.Application.Features.Commands.AddProductVariantImage;
using ProductVariantService.Application.Features.Commands.CreateProductVariant;
using ProductVariantService.Application.Features.Commands.ReorderProductVariantImages;
using ProductVariantService.Application.Features.Commands.UpdateProductVariantImageAlt;
using ProductVariantService.Application.Features.Commands.UpdateProductVariantInfo;

namespace ProductVariantService.Api.Grpc.Mappers;

internal static class ProductVariantGrpcMapper
{
    public static CreateProductVariantCommand ToCreateProductVariantCommand(CreateProductVariantRequest request) =>
        new(
            VendorId: Guid.Parse(request.VendorId),
            ProductId: Guid.Parse(request.ProductId),

            Dimensions: ToDimensions(request.Dimensions),
            Weight: ToWeight(request.Weight),

            Barcode: request.Barcode,
            SKU: request.Sku,

            Attributes: request.Attributes.ToDictionary(a => Guid.Parse(a.Key), a => a.Value));

    public static UpdateProductVariantInfoCommand ToUpdateProductVariantInfoCommand(UpdateProductVariantInfoRequest request) =>
        new(
            VendorId: Guid.Parse(request.VendorId),
            ProductVariantId: Guid.Parse(request.ProductVariantId),
            SKU: request.Sku,
            Barcode: request.Barcode,
            Dimensions: ToDimensions(request.Dimensions),
            Weight: ToWeight(request.Weight));
    
    public static AddProductVariantImageCommand ToAddProductVariantImageCommand(AddProductVariantImageRequest request) =>
        new(
            ProductVariantId: Guid.Parse(request.ProductVariantId),
            VendorId: Guid.Parse(request.VendorId),
            ImageUrl: request.Url,
            AltText: request.Alt);
    
    public static ReorderProductVariantImagesCommand ToReorderProductVariantImagesCommand(ReorderProductVariantImagesRequest request) =>
        new(
            ProductVariantId: Guid.Parse(request.ProductVariantId),
            VendorId: Guid.Parse(request.VendorId),
            ImageIds: [.. request.OrderedImageIds.Select(Guid.Parse)]);

    public static UpdateProductVariantImageAltCommand ToUpdateProductVariantImageAltCommand(UpdateProductVariantImageAltRequest request) =>
        new(
            ProductVariantId: Guid.Parse(request.ProductVariantId),
            VendorId: Guid.Parse(request.VendorId),
            ImageId: Guid.Parse(request.ImageId),
            AltText: request.NewAlt);

    private static Application.Dtos.DimensionsDto ToDimensions(DimensionsDto dto) =>
        new(
            dto.Length,
            dto.Width,
            dto.Height,
            dto.Unit);

    private static Application.Dtos.WeightDto ToWeight(WeightDto dto) =>
        new(
            dto.Value,
            dto.Unit);
}