using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Movie_Ticket_Booking.Models;

namespace Movie_Ticket_Booking.Service
{
    public class SeatService
    {
        private readonly IMongoCollection<Seat> _seatCollection;

        public SeatService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _seatCollection = database.GetCollection<Seat>("seat");
        }

        public async Task<List<SeatInfo>> GetAsync()
        {
            var pipeline = new BsonDocument[]
               {
                 new BsonDocument("$lookup",
                     new BsonDocument
                     {
                         { "from", "theatre" },
                         { "localField", "theatre" },
                         { "foreignField", "_id" },
                         { "as", "theatre" }
                     }
                  ),
                 new BsonDocument("$unwind", "$theatre"),
                 new BsonDocument("$project",
                     new BsonDocument
                     {
                         { "_id", 1 },
                         { "theatre._id", 1 },
                         { "theatre.name", 1 },
                         { "theatre.description", 1 },
                         { "row", 1 },
                         { "number", 1 },
                     }),
                     new BsonDocument("$sort",
                        new BsonDocument
                        {
                            { "theatre.name", 1 }
                        }
                    ),
             };

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _seatCollection.Aggregate<SeatInfo>(pipeline, options).ToListAsync();
            return result;
        }
        public async Task CreateAsync(Seat seat)
        {
            await _seatCollection.InsertOneAsync(seat);
            return;
        }

        public async Task<List<SeatInfo>> GetByIdAsync(string id)
        {
            var pipeline = new BsonDocument[]
               {
                   new BsonDocument("$match",
                        new BsonDocument
                        {

                            { "_id", new BsonObjectId(ObjectId.Parse(id)) }
                        }
                    ),
                 new BsonDocument("$lookup",
                     new BsonDocument
                     {
                         { "from", "theatre" },
                         { "localField", "theatre" },
                         { "foreignField", "_id" },
                         { "as", "theatre" }
                     }
                  ),
                 new BsonDocument("$unwind", "$theatre"),
                 new BsonDocument("$project",
                     new BsonDocument
                     {
                         { "_id", 1 },
                         { "theatre._id", 1 },
                         { "theatre.name", 1 },
                         { "theatre.description", 1 },
                         { "row", 1 },
                         { "number", 1 },
                     }),
             };

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _seatCollection.Aggregate<SeatInfo>(pipeline, options).ToListAsync();
            return result;
        }
        public async Task DeleteAsync(string id)
        {
            FilterDefinition<Seat> filter = Builders<Seat>.Filter.Eq("Id", id);
            await _seatCollection.DeleteOneAsync(filter);
            return;
        }
    }
}
