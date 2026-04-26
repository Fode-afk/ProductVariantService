using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;

namespace ProductService.Infrastructure.Data.Configurations;

internal sealed class ProductReadModelConfiguration : IEntityTypeConfiguration<ProductReadModel>
{
    public void Configure(EntityTypeBuilder<ProductReadModel> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductCardId)
            .IsRequired();

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

        builder.Property(x => x.Length);
        builder.Property(x => x.Width);
        builder.Property(x => x.Height);

        builder.Property(x => x.DimensionUnit)
            .HasMaxLength(10);

        builder.Property(x => x.Weight);

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

        builder.Property(x => x.IsDefault)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

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
