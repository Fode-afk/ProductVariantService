using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductVariantService.Domain.Models;

namespace ProductVariantService.Infrastructure.Data.Configurations;

internal sealed class ProductVariantReadModelConfiguration : IEntityTypeConfiguration<ProductVariantReadModel>
{
    public void Configure(EntityTypeBuilder<ProductVariantReadModel> builder)
    {
        builder.ToTable("ProductVariantReadModels", Schemas.ProductVariantRead);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SKU)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.NameNormalized)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Barcode)
            .HasMaxLength(50);

        builder.Property(x => x.DimensionUnit)
            .HasMaxLength(10);

        builder.Property(x => x.WeightUnit)
            .HasMaxLength(10);

        builder.Property(x => x.MainImage)
            .HasMaxLength(500);

        builder.Property(x => x.AttributesJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.TagsJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.ImagesJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.TagsFlat)
            .HasMaxLength(500);

        builder.Property(x => x.PriceAmount)
            .HasPrecision(18, 4);
        builder.Property(x => x.OldPriceAmount)
            .HasPrecision(18, 4);
        builder.Property(x => x.PriceUpdatedAt);

        builder.HasIndex(x => x.ProductCardId);

        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.NameNormalized);

        builder.HasIndex(x => x.SKU).IsUnique();

        builder.HasIndex(p => new { p.ProductCardId, p.IsDefault })
             .IsUnique()
             .HasFilter("[IsDefault] = 1");

        builder.HasIndex(x => x.IsDefault);

        builder.Property<byte[]>("RowVersion")
          .IsRowVersion()
          .IsConcurrencyToken();
    }
}
