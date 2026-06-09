using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductVariantService.Application.Interfaces.Data;
using ProductVariantService.Domain.Contexts;
using ProductVariantService.Domain.Errors;
using ProductVariantService.Domain.Models;
using ProductVariantService.Domain.Specifications.Common;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.Commands.CreateProductVariant;

public sealed class CreateProductVariantCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<CreateProductVariantCommand, IResult>
{
    public async Task<IResult> Handle(CreateProductVariantCommand request, CancellationToken cancellationToken)
    {
        var vendorSnapshot = await context.VendorSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VendorId == request.VendorId, cancellationToken);
        if (vendorSnapshot is null)
            return Fail(VendorSnapshotErrors.NotFound());

        var productSnapshot = await context.ProductSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProductId == request.ProductId, cancellationToken);
        if (productSnapshot is null)
            return Fail(ProductSnapshotErrors.NotFound());

        var ownershipCtx = new ProductVendorOwnershipContext(
           request.VendorId,
           productSnapshot.VendorId);

        var ownershipResult = ProductBelongsToVendorSpec.Instance.IsSatisfiedBy(ownershipCtx);
        if (ownershipResult.IsFailure)
            return ownershipResult;

        var characteristicIds = request.Attributes.Keys.ToList();
        var characteristicSnapshots = await context.CharacteristicSnapshots
            .AsNoTracking()
            .Where(c =>
                c.CategoryId == productSnapshot.CategoryId &&
                characteristicIds.Contains(c.CharacteristicId))
            .ToListAsync(cancellationToken);

        var missingIds = characteristicIds
            .Except(characteristicSnapshots.Select(c => c.CharacteristicId))
            .ToList();
        if (missingIds.Count > 0)
            return Fail(CharacteristicSnapshotErrors.NotFound());

        var unifyingChar = characteristicSnapshots.FirstOrDefault(c => c.IsUnifying);
        if (unifyingChar is not null)
            return Fail(CharacteristicSnapshotErrors.CannotUseUnifyingAsVariant());

        var existingVariants = await context.ProductVariants
            .Where(v => v.ProductId == request.ProductId)
            .ToListAsync(cancellationToken);

        var requestedPairs = request.Attributes
            .OrderBy(x => x.Key)
            .Select(x => (x.Key, x.Value))
            .ToList();

        var isDuplicate = existingVariants.Any(v =>
        {
            var existingPairs = v.Attributes
                .OrderBy(a => a.CharacteristicId)
                .Select(a => (a.CharacteristicId, a.Value.ToString()))
                .ToList();

            return existingPairs.SequenceEqual(requestedPairs);
        });

        if (isDuplicate)
            return Fail(ProductVariantErrors.DuplicateCombination());

        var buildResult = ProductVariantCreationDataBuilder.Build(request, characteristicSnapshots);
        if (buildResult.IsFailure)
            return buildResult;

        var data = buildResult.Value;

        var ctx = new ProductVariantCreationContext(
            vendorSnapshot.IsActive,
            productSnapshot.CanBeModified,
            data.Attributes);

        var result = ProductVariant.Create(
            ctx,
            data,
            request.ProductId,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        context.ProductVariants.Add(result.Value);

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return Fail(ProductVariantErrors.AlreadyExists());
        }

        return Ok();
    }
}