using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;

namespace ProductService.Infrastructure.Data.Configurations;

internal sealed class ProductCardSnapshotConfiguration : IEntityTypeConfiguration<ProductCardSnapshot>
{
    public void Configure(EntityTypeBuilder<ProductCardSnapshot> builder)
    {
        builder.HasKey(x => x.ProductCardId);
        builder.HasIndex(x => x.VendorId);

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}