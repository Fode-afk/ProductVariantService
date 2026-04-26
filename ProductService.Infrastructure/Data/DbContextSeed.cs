using ProductService.Domain.Models;
using ProductService.Domain.ValueObjects;

namespace ProductService.Infrastructure.Data;

internal static class DbContextSeed
{
    public static async Task SeedAsync(AppDbContext context)
    {
        /*if (!context.Products.Any())
        {
            var now = DateTimeOffset.UtcNow;

            var products = new List<Product>
            {
                Product.Create(
                    Guid.NewGuid(),
                    Sku.Create("IP15").Value,
                    Name.Create("iPhone 15").Value,
                    Dimensions.Create(146.7, 71.5, 7.4, DimensionUnit.Mm).Value,
                    Weight.Create(0.2, WeightUnit.Kilogram).Value,
                    Barcode.Create("1234567890123").Value,
                    true,
                    [
                        ProductAttribute.Create("Color", "White").Value,
                        ProductAttribute.Create("Memory", "128GB").Value
                    ],
                    [
                        Tag.Create("phone").Value,
                        Tag.Create("apple").Value
                    ],
                    now).Value,

                Product.Create(
                    Guid.NewGuid(),
                    Sku.Create("IP15PM").Value,
                    Name.Create("iPhone 15 Pro Max").Value,
                    Dimensions.Create(159.9, 76.7, 8.3, DimensionUnit.Mm).Value,
                    Weight.Create(0.221, WeightUnit.Kilogram).Value,
                    Barcode.Create("1234567890124").Value,
                    true,
                    [
                        ProductAttribute.Create("Color", "Black").Value,
                        ProductAttribute.Create("Memory", "256GB").Value
                    ],
                    [
                        Tag.Create("phone").Value,
                        Tag.Create("apple").Value,
                        Tag.Create("premium").Value
                    ],
                    now).Value,

                Product.Create(
                    Guid.NewGuid(),
                    Sku.Create("S24").Value,
                    Name.Create("Samsung Galaxy S24").Value,
                    Dimensions.Create(147.0, 70.6, 7.6, DimensionUnit.Mm).Value,
                    Weight.Create(0.168, WeightUnit.Kilogram).Value,
                    Barcode.Create("2234567890123").Value,
                    true,
                    [
                        ProductAttribute.Create("Color", "Gray").Value,
                        ProductAttribute.Create("Memory", "128GB").Value
                    ],
                    [
                        Tag.Create("phone").Value,
                        Tag.Create("samsung").Value
                    ],
                    now).Value,

                Product.Create(
                    Guid.NewGuid(),
                    Sku.Create("MBP16").Value,
                    Name.Create("MacBook Pro 16").Value,
                    Dimensions.Create(355.7, 248.1, 16.8, DimensionUnit.Mm).Value,
                    Weight.Create(2.1, WeightUnit.Kilogram).Value,
                    Barcode.Create("3234567890123").Value,
                    true,
                    [
                        ProductAttribute.Create("RAM", "16GB").Value,
                        ProductAttribute.Create("Storage", "512GB").Value
                    ],
                    [
                        Tag.Create("laptop").Value,
                        Tag.Create("apple").Value
                    ],
                    now).Value,

                Product.Create(
                    Guid.NewGuid(),
                    Sku.Create("AIRPODS").Value,
                    Name.Create("AirPods Pro").Value,
                    Dimensions.Create(30.9, 21.8, 24.0, DimensionUnit.Mm).Value,
                    Weight.Create(0.056, WeightUnit.Kilogram).Value,
                    Barcode.Create("4234567890123").Value,
                    true,
                    [
                        ProductAttribute.Create("Type", "In-ear").Value,
                        ProductAttribute.Create("NoiseCancelling", "Yes").Value
                    ],
                    [
                        Tag.Create("audio").Value,
                        Tag.Create("apple").Value
                    ],
                    now).Value,

                Product.Create(
                    Guid.NewGuid(),
                    Sku.Create("SONYWH").Value,
                    Name.Create("Sony WH-1000XM5").Value,
                    Dimensions.Create(250, 200, 80, DimensionUnit.Mm).Value,
                    Weight.Create(0.25, WeightUnit.Kilogram).Value,
                    Barcode.Create("5234567890123").Value,
                    true,
                    [
                        ProductAttribute.Create("Type", "Over-ear").Value,
                        ProductAttribute.Create("Wireless", "Yes").Value
                    ],
                    [
                        Tag.Create("audio").Value,
                        Tag.Create("sony").Value
                    ],
                    now).Value
            };

            await context.Products.AddRangeAsync(products);
        } */

        if (!context.ProductCardSnapshots.Any())
        {
            var snapshots = new List<ProductCardSnapshot>();

            for (int i = 0; i < 10; i++)
                snapshots.Add(
                    new ProductCardSnapshot
                    {
                        ProductCardId = Guid.NewGuid(),
                        VendorId = Guid.NewGuid(),
                        Status = ProductCardStatus.Draft
                    });

            await context.ProductCardSnapshots.AddRangeAsync(snapshots);
        }

        await context.SaveChangesAsync();
    }
}