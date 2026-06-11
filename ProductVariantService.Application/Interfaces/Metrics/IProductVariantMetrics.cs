namespace ProductVariantService.Application.Interfaces.Metrics;

public interface IProductVariantMetrics
{
    void RecordVariantCreated();
    void RecordVariantDeleted();

    void SetActiveVariantsCount(int count);

    void RecordSnapshotNotFound(string snapshotType);
    void RecordSnapshotOutdated(string handlerName);
}