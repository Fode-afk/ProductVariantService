using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductVariantService.Domain.Models;
using ProductVariantService.Domain.ValueObjects;

namespace ProductVariantService.Infrastructure.Data.Configurations;

internal sealed class ProductVariantImageConfiguration : IEntityTypeConfiguration<ProductVariantImage>
{
    public void Configure(EntityTypeBuilder<ProductVariantImage> builder)
    {
        builder.ToTable("ProductVariantImages", Schemas.ProductVariantWrite);

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Url)
            .HasConversion(
                url => url.Value,
                value => ImageUrl.Create(value).Value);

        builder.Property(i => i.Alt)
            .HasMaxLength(AltText.MaxLength)
            .HasConversion(
                alt => alt.Value,
                value => AltText.Create(value).Value);

        builder.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}
