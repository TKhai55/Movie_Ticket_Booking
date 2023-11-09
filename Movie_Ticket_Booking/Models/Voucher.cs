using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Movie_Ticket_Booking.Models
{
    public class Voucher
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string name { get; set; } = null!;

        //  [BsonElement("items")]
        //   [JsonPropertyName("items")]
        public string description { get; set; } = null!;
        public int value { get; set; } = 0!;
    }
}
