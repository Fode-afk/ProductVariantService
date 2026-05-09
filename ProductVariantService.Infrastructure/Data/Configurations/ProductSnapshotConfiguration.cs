using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductVariantService.Domain.Snapshots;

namespace ProductVariantService.Infrastructure.Data.Configurations;

internal sealed class ProductSnapshotConfiguration : IEntityTypeConfiguration<ProductSnapshot>
{
    public void Configure(EntityTypeBuilder<ProductSnapshot> builder)
    {
        builder.ToTable("ProductSnapshots", Schemas.ProductVariantWrite);

        builder.HasKey(x => x.ProductId);

        builder.HasIndex(x => x.VendorId);
        builder.HasIndex(x => x.CategoryId);

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}