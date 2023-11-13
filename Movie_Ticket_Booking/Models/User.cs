    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Bson;

    namespace Movie_Ticket_Booking.Models
    {
        public class User
        {
            [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }

            public string account { get; set; } = null!;
            public string password { get; set; } = null!;


        }
    }
