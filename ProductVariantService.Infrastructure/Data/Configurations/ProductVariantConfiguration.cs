using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.ValueObjects;
using ProductVariantService.Domain.Models;
using ProductVariantService.Domain.ValueObjects;

namespace ProductVariantService.Infrastructure.Data.Configurations;

internal sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants", Schemas.ProductVariantWrite);

        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.ProductId);
        builder.HasIndex(p => p.SKU).IsUnique();
        builder.HasIndex(p => p.Barcode).IsUnique();

        builder.Property(p => p.SKU)
            .HasConversion(
                sku => sku.Value,
                value => Sku.Create(value).Value);

        builder.OwnsOne(p => p.Dimensions, d =>
        {
            d.Property(d => d.Length)
                .HasColumnName("DimensionsLength");

            d.Property(d => d.Width)
                .HasColumnName("DimensionsWidth");

            d.Property(d => d.Height)
                .HasColumnName("DimensionsHeight");

            d.Property(d => d.Unit)
                .HasColumnName("DimensionsUnit")
                .HasConversion(
                    unit => unit.Code,
                    code => DimensionUnit.From(code).Value);
        });

        builder.OwnsOne(p => p.Weight, w =>
        {
            w.Property(w => w.Value)
                .HasColumnName("Weight");

            w.Property(w => w.Unit)
                .HasColumnName("WeightUnit")
                .HasConversion(
                    unit => unit.Code,
                    code => WeightUnit.From(code).Value);
        });

        builder.Property(p => p.Barcode)
            .HasConversion(
                barcode => barcode.Value,
                value => Barcode.Create(value).Value);

        builder.OwnsMany(p => p.Attributes, a =>
        {
            a.ToTable("ProductVariantAttributes", Schemas.ProductVariantWrite);

            a.WithOwner().HasForeignKey("ProductVariantId");

            a.Property<int>("Id");
            a.HasKey("Id");

            a.Property(x => x.CharacteristicId)
                .IsRequired();

            a.OwnsOne(x => x.Name, n =>
            {
                n.Property(x => x.Value)
                    .HasColumnName("Name")
                    .HasMaxLength(AttributeName.MaxLength)
                    .IsRequired();
            });

            a.OwnsOne(x => x.Value, v =>
            {
                v.Property(x => x.Value)
                    .HasColumnName("Value")
                    .HasMaxLength(AttributeValue.MaxLength)
                    .IsRequired();
            });

            a.Property(x => x.CharType)
                .HasColumnName("CharType")
                .IsRequired();

            a.OwnsOne(x => x.GroupName, g =>
            {
                g.Property(x => x.Value)
                    .HasColumnName("GroupName")
                    .HasMaxLength(AttributeGroupName.MaxLength);
            });
        });

        builder.Navigation(p => p.Attributes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(p => p.Images)
            .WithOne()
            .HasForeignKey("ProductVariantId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(p => p.Images)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}
