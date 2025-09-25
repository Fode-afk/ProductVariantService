using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductService.Models
{
    public class Card
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CardId { get; set; }

        public string OwnerId { get; set; }

        public List<string> ProductIds { get; set; }
    }
}
