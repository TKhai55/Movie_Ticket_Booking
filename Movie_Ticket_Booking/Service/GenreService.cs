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

        public async Task<List<Genre>> GetByIdAsync(string id)
        {
            var pipeline = new BsonDocument[]
            {
                 new BsonDocument("$match",
                        new BsonDocument
                        {

                            { "_id", new BsonObjectId(ObjectId.Parse(id)) }
                        }
                    ),
                };

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _genreCollection.Aggregate<Genre>(pipeline, options).ToListAsync();
            return result;
        }

        public async Task UpdateAsync(string id, Genre updatedGenre)
        {
            var filter = Builders<Genre>.Filter.Eq(genre => genre.Id, id);
            var updateBuilder = Builders<Genre>.Update;

            var updateDefinition = updateBuilder.Set(genre => genre.Id, id);

            if (updatedGenre.name != null)
                updateDefinition = updateDefinition.Set(genre => genre.name, updatedGenre.name);

            
            await _genreCollection.UpdateOneAsync(filter, updateDefinition);

        }
        public async Task DeleteAsync(string id)
        {
            FilterDefinition<Genre> filter = Builders<Genre>.Filter.Eq("Id", id);
            await _genreCollection.DeleteOneAsync(filter);
            return;
        }
    }
}
