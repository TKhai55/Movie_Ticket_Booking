using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Movie_Ticket_Booking.Models
{
    public class MovieWithGenre
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string name { get; set; } = null!;
        public string studio { get; set; } = null!;
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime publishDate { get; set; }
        public List<Genre> genre { get; set; } = null!;
        public string type { get; set; } = null!;
        public string actors { get; set; } = null!;
        public string director { get; set; } = null!;
        public string description { get; set; } = null!;
        public string image { get; set; } = null!;
        public string trailer { get; set; } = null!;
        public int duration { get; set; } = 0!;
        public int profit { get; set; } = 0!;
    }
}
