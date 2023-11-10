using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Movie_Ticket_Booking.Models;

namespace Movie_Ticket_Booking.Service
{

    public class MovieService
    {
        private readonly IMongoCollection<Movie> _movieCollection;

        public MovieService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _movieCollection = database.GetCollection<Movie>("movie");
        }

        public async Task<List<Movie>> GetAsync()
        {
            return await _movieCollection.Find(new BsonDocument()).ToListAsync();
        }
        public async Task CreateAsync(Movie movie)
        {
            await _movieCollection.InsertOneAsync(movie);
            return;
        }

        public async Task DeleteAsync(string id)
        {
            FilterDefinition<Movie> filter = Builders<Movie>.Filter.Eq("Id", id);
            await _movieCollection.DeleteOneAsync(filter);
            return;
        }
    }
}
