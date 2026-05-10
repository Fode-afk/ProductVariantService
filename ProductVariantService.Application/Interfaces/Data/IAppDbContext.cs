using Microsoft.EntityFrameworkCore;
using ProductVariantService.Domain.Models;
using ProductVariantService.Domain.Snapshots;

namespace ProductVariantService.Application.Interfaces.Data;

public interface IAppDbContext
{
    DbSet<ProductVariant> ProductVariants { get; }
    DbSet<ProductVariantImage> ProductVariantImages { get; }

    DbSet<ProductSnapshot> ProductSnapshots { get; }
    DbSet<VendorSnapshot> VendorSnapshots { get; }
    DbSet<CharacteristicSnapshot> CharacteristicSnapshots { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
