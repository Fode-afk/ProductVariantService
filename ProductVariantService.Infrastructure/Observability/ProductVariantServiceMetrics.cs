using ProductVariantService.Application.Interfaces.Metrics;
using System.Diagnostics.Metrics;

namespace ProductVariantService.Infrastructure.Observability;

public sealed class ProductVariantServiceMetrics : IDisposable, IProductVariantMetrics
{
    public const string MeterName = "ProductVariantService";
    private readonly Meter _meter;

    private readonly Counter<long> _variantsCreated;
    private readonly Counter<long> _variantsDeleted;
    private int _activeVariantsCount_value;
    private readonly ObservableGauge<int> _activeVariantsCount;

    private readonly Counter<long> _handlerErrors;
    private readonly Histogram<double> _handlerDuration;

    private readonly Counter<long> _snapshotNotFound;
    private readonly Counter<long> _snapshotOutdated;

    public ProductVariantServiceMetrics()
    {
        _meter = new Meter(MeterName);

        _variantsCreated = _meter.CreateCounter<long>(
            "product.variants.created");

        _variantsDeleted = _meter.CreateCounter<long>(
            "product.variants.deleted");

        _activeVariantsCount = _meter.CreateObservableGauge(
            "product.variants.active",
            () => _activeVariantsCount_value);

        _handlerErrors = _meter.CreateCounter<long>(
            "product.variants.handlers.errors");

        _handlerDuration = _meter.CreateHistogram<double>(
            "product.variants.handlers.duration",
            unit: "ms");

        _snapshotNotFound = _meter.CreateCounter<long>(
            "product.variants.snapshots.not_found",
            description: "Snapshot missing when projection arrived — possible race condition");

        _snapshotOutdated = _meter.CreateCounter<long>(
            "product.variants.snapshots.outdated",
            description: "Projection skipped because version is outdated");
    }

    public void RecordVariantCreated() => _variantsCreated.Add(1);

    public void RecordVariantDeleted() => _variantsDeleted.Add(1);

    public void SetActiveVariantsCount(int count) =>
        _activeVariantsCount_value = count;

    public void RecordHandlerError(string handlerName, string handlerType) =>
        _handlerErrors.Add(1,
            new KeyValuePair<string, object?>("handler", handlerName),
            new KeyValuePair<string, object?>("type", handlerType));

    public void RecordHandlerDuration(double ms, string handlerName, string handlerType) =>
        _handlerDuration.Record(ms,
            new KeyValuePair<string, object?>("handler", handlerName),
            new KeyValuePair<string, object?>("type", handlerType));

    public void RecordSnapshotNotFound(string snapshotType) =>
        _snapshotNotFound.Add(1, new KeyValuePair<string, object?>("type", snapshotType));

    public void RecordSnapshotOutdated(string handlerName) =>
        _snapshotOutdated.Add(1, new KeyValuePair<string, object?>("handler", handlerName));

    public void Dispose() => _meter.Dispose();
}