using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Movie_Ticket_Booking.Models
{
    public class Voucher
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        public string name { get; set; } = null!;

        public string code { get; set; } = null!;

        public string description { get; set; } = null!;
        public int value { get; set; } = 0!;
        public Boolean isActive { get; set; } = true!;
    }
}
