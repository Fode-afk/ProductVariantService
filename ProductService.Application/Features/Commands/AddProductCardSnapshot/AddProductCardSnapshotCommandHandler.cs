using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductService.Application.Interfaces.Data;
using ProductService.Domain.Errors;
using ProductService.Domain.Models;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Application.Features.Commands.AddProductCardSnapshot;

public sealed class AddProductCardSnapshotCommandHandler(IAppDbContext context) : IRequestHandler<AddProductCardSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(AddProductCardSnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.ProductCardSnapshots
            .AnyAsync(x => x.ProductCardId == request.ProductCardId, cancellationToken);

        if (exists)
            return Fail(ProductCardSnapshotErrors.AlreadyExists());

        context.ProductCardSnapshots.Add(
            new ProductCardSnapshot
            { 
                ProductCardId = request.ProductCardId,
                VendorId = request.VendorId
            });

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
