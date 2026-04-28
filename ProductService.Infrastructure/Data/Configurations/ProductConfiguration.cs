using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using migApp.Shared.Domain.ValueObjects;
using ProductService.Domain.Models;
using ProductService.Domain.ValueObjects;

namespace ProductService.Infrastructure.Data.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.ProductCardId);
        builder.HasIndex(p => p.SKU).IsUnique();
        builder.HasIndex(p => p.Barcode).IsUnique();
        builder.HasIndex(p => new { p.ProductCardId, p.IsDefault })
             .IsUnique()
             .HasFilter("[IsDefault] = 1");

        builder.Property(p => p.SKU)
            .HasConversion(
                sku => sku.Value,
                value => Sku.Create(value).Value);

        builder.Property(p => p.Name)
            .HasMaxLength(Name.MaxLength)
            .HasConversion(
                name => name.Value,
                value => Name.Create(value).Value);

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

        builder.Property(x => x.PriceSnapshot)
            .HasConversion(
                price => price == null ? (decimal?)null : price.Amount,
                value => value == null ? null : Money.Create(value.Value, Currency.USD).Value);

        builder.Property(x => x.OldPriceSnapshot)
            .HasConversion(
                price => price == null ? (decimal?)null : price.Amount,
                value => value == null ? null : Money.Create(value.Value, Currency.USD).Value);

        builder.OwnsMany(p => p.Attributes, a =>
        {
            a.WithOwner().HasForeignKey("ProductId");

            a.ToTable("ProductAttributes");

            a.Property<int>("Id");
            a.HasKey("Id");

            a.OwnsOne(x => x.Name, n =>
            {
                n.Property(x => x.Value)
                    .HasColumnName("Name")
                    .HasMaxLength(AttributeName.MaxLength);
            });

            a.OwnsOne(x => x.Value, v =>
            {
                v.Property(x => x.Value)
                    .HasColumnName("Value")
                    .HasMaxLength(AttributeValue.MaxLength);
            });
        });

        builder.OwnsMany(p => p.Tags, t =>
        {
            t.WithOwner().HasForeignKey("ProductId");

            t.ToTable("ProductTags");

            t.Property<int>("Id");
            t.HasKey("Id");

            t.Property(x => x.Value)
                .HasColumnName("Tag")
                .HasMaxLength(Tag.MaxLength);
        });

        builder.Property(x => x.RowVersion)
           .IsRowVersion()
           .IsConcurrencyToken();

        builder.Navigation(p => p.Tags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(p => p.Attributes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(p => p.Images)
            .WithOne()
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
