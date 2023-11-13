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

        public async Task<List<Seat>> GetAsync()
        {
            return await _seatCollection.Find(new BsonDocument()).ToListAsync();
        }
        public async Task CreateAsync(Seat seat)
        {
            await _seatCollection.InsertOneAsync(seat);
            return;
        }

        public async Task DeleteAsync(string id)
        {
            FilterDefinition<Seat> filter = Builders<Seat>.Filter.Eq("Id", id);
            await _seatCollection.DeleteOneAsync(filter);
            return;
        }
    }
}
