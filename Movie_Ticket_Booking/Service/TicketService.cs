using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Movie_Ticket_Booking.Models;

namespace Movie_Ticket_Booking.Service
{
    public class TicketService
    {
        private readonly IMongoCollection<Ticket> _ticketCollection;

        public TicketService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _ticketCollection = database.GetCollection<Ticket>("ticket");
        }

        public async Task<List<Ticket>> GetAsync()
        {
            return await _ticketCollection.Find(new BsonDocument()).ToListAsync();
        }
        public async Task CreateAsync(Ticket ticket)
        {
            await _ticketCollection.InsertOneAsync(ticket);
            return;
        }

        public async Task DeleteAsync(string id)
        {
            FilterDefinition<Ticket> filter = Builders<Ticket>.Filter.Eq("Id", id);
            await _ticketCollection.DeleteOneAsync(filter);
            return;
        }
    }
}
