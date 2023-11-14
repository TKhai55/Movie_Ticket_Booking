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
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime updatedAt { get; set; }
        public User creator { get; set; }
    }
}
