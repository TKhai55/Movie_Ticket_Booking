﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Movie_Ticket_Booking.Models
{
    public class News
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]

        public string creator { get; set; } = null!;
        public string title { get; set; } = null!;
        public string content { get; set; } = null!;
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime createdAt { get; set; }
        public News()
        {
            createdAt = DateTime.UtcNow;
        }
    }
}
