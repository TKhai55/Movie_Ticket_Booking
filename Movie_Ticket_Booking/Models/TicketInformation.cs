using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Movie_Ticket_Booking.Models
{
    public class TicketInformation
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string schedule { get; set; } = null!;
        public Seat seat { get; set; } = null!;
        public Voucher? voucher { get; set; } = null!;
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public int price { get; set; } = 0!;
    }
}
