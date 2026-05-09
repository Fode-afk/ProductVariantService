using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductVariantService.Domain.Snapshots;

namespace ProductVariantService.Infrastructure.Data.Configurations;

internal sealed class VendorSnapshotConfiguration : IEntityTypeConfiguration<VendorSnapshot>
{
    public void Configure(EntityTypeBuilder<VendorSnapshot> builder)
    {
        builder.ToTable("VendorSnapshots", Schemas.ProductVariantWrite);

        builder.HasKey(v => v.VendorId);

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}
