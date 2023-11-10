using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Movie_Ticket_Booking.Models;

namespace Movie_Ticket_Booking.Service
{
    public class GenreService
    {
        private readonly IMongoCollection<Genre> _genreCollection;

        public GenreService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _genreCollection = database.GetCollection<Genre>("genre");
        }

        public async Task<List<Genre>> GetAsync()
        {
            return await _genreCollection.Find(new BsonDocument()).ToListAsync();
        }
        public async Task CreateAsync(Genre genre)
        {
            await _genreCollection.InsertOneAsync(genre);
            return;
        }

        public async Task DeleteAsync(string id)
        {
            FilterDefinition<Genre> filter = Builders<Genre>.Filter.Eq("Id", id);
            await _genreCollection.DeleteOneAsync(filter);
            return;
        }
    }
}
