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
        [BsonRepresentation(BsonType.ObjectId)]
        public string voucher { get; set; } = null!;
    }
}
