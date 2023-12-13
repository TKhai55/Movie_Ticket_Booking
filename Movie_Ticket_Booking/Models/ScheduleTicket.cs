using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Movie_Ticket_Booking.Models
{
    public class ScheduleTicket
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime startTime { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime endTime { get; set; }

        public Movie movie { get; set; } = null!;
        public Theatre theatre { get; set; } = null!;
        public int price { get; set; } = 0!;
        public int total { get; set; } = 0!;
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> bookedSeat { get; set; } = null!;
    }
}
