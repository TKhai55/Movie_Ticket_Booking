using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Movie_Ticket_Booking.Models
{
    public class Schedule
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime startTime { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime endTime { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string movie { get; set; } = null!;
        [BsonRepresentation(BsonType.ObjectId)]
        public string theatre { get; set; } = null!;
        public int price { get; set; } = 0!;
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> bookedSeat { get; set; } = null!;
    }
}
