namespace ProductVariantService.Application.Interfaces.Metrics;

public interface IProductVariantMetrics
{
    void SetActiveVariantsCount(int count);

    void RecordSnapshotNotFound(string snapshotType);
    void RecordSnapshotOutdated(string handlerName);
    void RecordHandlerError(string handlerName, string handlerType);
    void RecordHandlerDuration(double ms, string handlerName, string handlerType);
}