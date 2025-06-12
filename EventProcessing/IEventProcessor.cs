namespace ProductService.EventProcessing
{
    public interface IEventProcessor
    {
        Task ProcessEventAsync(string message);
    }
}
