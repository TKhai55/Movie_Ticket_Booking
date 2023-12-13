using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Movie_Ticket_Booking.Models
{
    public class ScheduleFullinfo
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime startTime { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime endTime { get; set; }
 
        public MovieWithGenre movie { get; set; } = null!;
        public Theatre theatre { get; set; } = null!;
        public int price { get; set; } = 0!;
        public int total { get; set; } = 0!;
        public List<TicketSchedule>? bookedSeat { get; set; } = null!;
        
    }
}
