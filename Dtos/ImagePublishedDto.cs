using ProductService.EventProcessing;

namespace ProductService.Dtos
{
    public class ImagePublishedDto
    {
        public EventType Event { get; set; }
        public ServicesEnum Service { get; set; }
        public string Id { get; set; }
        public string Url { get; set; }
    }
}
