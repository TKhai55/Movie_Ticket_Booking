using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Movie_Ticket_Booking.Models
{
    public class Genre
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string name { get; set; } = null!;
    }
}
