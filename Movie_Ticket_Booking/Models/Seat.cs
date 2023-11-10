using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Movie_Ticket_Booking.Models
{
    public class Seat
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]

        public string theatre { get; set; } = null!;
        public string row { get; set; } = null!;
        public int number { get; set; } = 0!;
    }
}
