using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Movie_Ticket_Booking.Models
{
    public class News
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]

        public string creator { get; set; } = null!;
        public string title { get; set; } = null!;
        public string content { get; set; } = null!;
        public string imageURL { get; set; } = null!;
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime createdAt { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime updatedAt { get; set; } 
        public News()
        {
            createdAt = DateTime.UtcNow;
            updatedAt = DateTime.UtcNow;
        }
    }
}
