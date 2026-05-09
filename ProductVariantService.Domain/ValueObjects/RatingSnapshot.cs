using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using ProductVariantService.Domain.Errors;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Domain.ValueObjects;

public sealed class RatingSnapshot : ValueObject
{
    public decimal Avg { get; }
    public int Count { get; }

    private RatingSnapshot(decimal avg, int count)
    {
        Avg = avg;
        Count = count;
    }

    public static RatingSnapshot Empty => new(0, 0);

    public static IResult<RatingSnapshot> Create(decimal avg, int count)
    {
        if (count > 0 && (avg < 1 || avg > 5))
            return Fail<RatingSnapshot>(RatingSnapshotErrors.OutOfRange());

        if (count < 0)
            return Fail<RatingSnapshot>(RatingSnapshotErrors.InvalidCount());

        return Ok(new RatingSnapshot(avg, count));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Avg;
        yield return Count;
    }

    public override string ToString()
        => $"{Avg} | {Count}";

    public static implicit operator string(RatingSnapshot ratingSnapshot) => ratingSnapshot.ToString();
}
