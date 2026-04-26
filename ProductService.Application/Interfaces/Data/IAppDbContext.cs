using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Models;

namespace ProductService.Application.Interfaces.Data;

public interface IAppDbContext
{
    DbSet<Product> Products { get; }
    DbSet<ProductImage> ProductImages { get; }
    DbSet<ProductReadModel> ProductReadModels { get; }

    DbSet<ProductCardSnapshot> ProductCardSnapshots { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
