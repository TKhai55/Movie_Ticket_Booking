using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Movie_Ticket_Booking.Models
{
    public class Ticket
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string schedule { get; set; } = null!;
        [BsonRepresentation(BsonType.ObjectId)]
        public string seat { get; set; } = null!;
        public string voucher { get; set; } = null!;
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime createdAt { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime updatedAt { get; set; }
        public int price { get; set; } = 0!;
        public Ticket()
        {
            createdAt = DateTime.UtcNow;
            updatedAt = DateTime.UtcNow;
        }
    }
}
