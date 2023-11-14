using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Movie_Ticket_Booking.Models
{
    public class NewsWithCreator
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]

        public string title { get; set; } = null!;
        public string content { get; set; } = null!;
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime createdAt { get; set; }
        public User creatorInfo { get; set; }
    }
}
