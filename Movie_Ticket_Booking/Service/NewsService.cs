using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Movie_Ticket_Booking.Models;

namespace Movie_Ticket_Booking.Service
{
    public class NewsService
    {
        private readonly IMongoCollection<News> _newsCollection;

        public NewsService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _newsCollection = database.GetCollection<News>("news");
        }

        public async Task<List<News>> GetAsync()
        {
            return await _newsCollection.Find(new BsonDocument()).ToListAsync();
        }
        public async Task CreateAsync(News news)
        {
            await _newsCollection.InsertOneAsync(news);
            return;
        }

        public async Task DeleteAsync(string id)
        {
            FilterDefinition<News> filter = Builders<News>.Filter.Eq("Id", id);
            await _newsCollection.DeleteOneAsync(filter);
            return;
        }
    }
}
