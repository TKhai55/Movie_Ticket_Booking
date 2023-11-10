using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Movie_Ticket_Booking.Models
{
    public class Theatre
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string name { get; set; } = null!;
        public string description { get; set; } = null!;
    }
}
