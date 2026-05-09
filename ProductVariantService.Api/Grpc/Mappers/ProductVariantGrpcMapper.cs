using ProductVariantService.Api.Grpc.V1.Protos;
using ProductVariantService.Application.Features.Commands.CreateProductVariant;

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

    //public static UpdateProductInfoCommand ToUpdateProductInfoCommand(UpdateProductInfoRequest request) =>
    //    new(
    //        VendorId: Guid.Parse(request.VendorId),
    //        ProductId: Guid.Parse(request.ProductId),
    //        Name: request.Name,
    //        Dimensions: ToDimensions(request.Dimensions),
    //        Weight: ToWeight(request.Weight));
    //
    //public static AddProductImageCommand ToAddProductImageCommand(AddProductImageRequest request) =>
    //    new(
    //        ProductId: Guid.Parse(request.ProductId),
    //        VendorId: Guid.Parse(request.VendorId),
    //        ImageUrl: request.Url,
    //        AltText: request.Alt,
    //        IsMain: request.IsMain);
    //
    //public static RemoveProductImageCommand ToRemoveProductImageCommand(RemoveProductImageRequest request) =>
    //    new(
    //        ProductId: Guid.Parse(request.ProductId),
    //        VendorId: Guid.Parse(request.VendorId),
    //        Url: request.Url);
    //
    //public static ChangeProductImageOrderCommand ToChangeProductImageOrderCommand(ChangeProductImageOrderRequest //request) =>
    //    new(
    //        ProductId: Guid.Parse(request.ProductId),
    //        VendorId: Guid.Parse(request.VendorId),
    //        Url: request.Url,
    //        NewOrder: request.NewSortOrder);
    //
    //public static SetMainProductImageCommand ToSetMainProductImageCommand(SetMainProductImageRequest request) =>
    //    new(
    //        ProductId: Guid.Parse(request.ProductId),
    //        VendorId: Guid.Parse(request.VendorId),
    //        Url: request.Url);
    //
    //public static UpdateProductImageAltCommand ToUpdateProductImageAltCommand(UpdateProductImageAltRequest /request) /=>
    //    new(
    //        ProductId: Guid.Parse(request.ProductId),
    //        VendorId: Guid.Parse(request.VendorId),
    //        Url: request.Url,
    //        AltText: request.NewAlt);

    public static ProductDto FromDto(Application.Dtos.ProductDto product) => 
        new()
        {
            Id = product.Id.ToString(),
            Sku = product.SKU,
            Name = product.Name,
            Barcode = product.Barcode,

            Dimensions = FromDimensions(product.Dimensions),
            Weight = FromWeight(product.Weight),

            PriceMinor = product.PriceMinorAmount,
            OldPriceMinor = product.OldPriceMinorAmount,

            StockStatus = (StockStatus)product.Status,
            AvailableQuantity = product.AvailableQuantity,

            Attributes = { product.Attributes },
            Tags = { product.Tags },
            Images = { product.Images.Select(FromProductImage) }
        };

    private static Application.Dtos.DimensionsDto ToDimensions(DimensionsDto dto) =>
        new(
            dto.Length,
            dto.Width,
            dto.Height,
            dto.Unit);

    private static DimensionsDto FromDimensions(Application.Dtos.DimensionsDto dto) =>
       new()
       {
           Length = dto.Length,
           Width = dto.Width,
           Height = dto.Height,
           Unit = dto.Unit
       };

    private static Application.Dtos.WeightDto ToWeight(WeightDto dto) =>
        new(
            dto.Value,
            dto.Unit);

    private static WeightDto FromWeight(Application.Dtos.WeightDto dto) =>
        new()
        {
            Value = dto.Value,
            Unit = dto.Unit
        };

    private static Application.Dtos.ProductImageDto ToProductImage(ProductImageDto image) =>
       new(
           image.Url,
           image.Alt,
           image.IsMain,
           image.SortOrder);

    private static ProductImageDto FromProductImage(Application.Dtos.ProductImageDto image) =>
        new()
        {
            Url = image.Url,
            Alt = image.Alt,
            IsMain = image.IsMain,
            SortOrder = image.SortOrder
        };
}