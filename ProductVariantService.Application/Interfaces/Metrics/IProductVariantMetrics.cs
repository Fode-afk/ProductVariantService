namespace ProductVariantService.Application.Interfaces.Metrics;

public interface IProductVariantMetrics
{
    void RecordSnapshotNotFound(string snapshotType);
    void RecordSnapshotOutdated(string handlerName);
}