namespace ProductService.Dtos
{
    public class CardPublishedDto
    {
        public string CardId { get; set; }

        public string OwnerId { get; set; }

        public string Name { get; set; }

        public List<string> ProductIds { get; set; }
    }
}
