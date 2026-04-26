using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;
using ProductService.Domain.ValueObjects;

namespace ProductService.Infrastructure.Data.Configurations;

internal sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
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
