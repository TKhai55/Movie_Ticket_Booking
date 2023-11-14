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

        public async Task<List<MovieWithGenre>> GetAsync()
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$lookup",
                    new BsonDocument    
                    {
                        { "from", "genre" },
                        { "localField", "genre" },
                        { "foreignField", "_id" },
                        { "as", "genre" }
                    }
                ),
                new BsonDocument("$project",
                    new BsonDocument
                    {
                        { "_id", 1 },
                        { "name", 1 },
                        { "studio", 1 },
                        { "publishDate", 1 },
                        { "genre._id", 1 },
                        { "genre.name", 1 },
                        { "type", 1 },
                        { "actors", 1 },
                        { "director", 1 },
                        { "description", 1 },
                        { "image", 1 },
                        { "trailer", 1 },
                        { "duration", 1 },
                        
                    }
                ),
                new BsonDocument("$sort",
                        new BsonDocument
                        {
                            { "publishDate", 1 }
                        }
                    ),
            };


            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _movieCollection.Aggregate<MovieWithGenre>(pipeline, options).ToListAsync();
            return result;
        }

        public async Task<List<MovieWithGenre>> GetByIdAsync(string id)
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
                        { "from", "genre" },
                        { "localField", "genre" },
                        { "foreignField", "_id" },
                        { "as", "genre" }
                    }
                ),
                new BsonDocument("$project",
                    new BsonDocument
                    {
                        { "_id", 1 },
                        { "name", 1 },
                        { "studio", 1 },
                        { "publishDate", 1 },
                        { "genre._id", 1 },
                        { "genre.name", 1 },
                        { "type", 1 },
                        { "actors", 1 },
                        { "director", 1 },
                        { "description", 1 },
                        { "image", 1 },
                        { "trailer", 1 },
                        { "duration", 1 },

                    }
                )
            };


            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _movieCollection.Aggregate<MovieWithGenre>(pipeline, options).ToListAsync();
            return result;
        }
        public async Task CreateAsync(Movie movie)
        {
            // Đảm bảo định dạng ngày và giờ là UTC trước khi lưu trữ
            movie.publishDate = DateTime.SpecifyKind(movie.publishDate, DateTimeKind.Utc);
            await _movieCollection.InsertOneAsync(movie);
            return;
        }

        public async Task UpdateAsync(string id, Movie updatedMovie)
        {
            var filter = Builders<Movie>.Filter.Eq(movie => movie.Id, id);
            var updateBuilder = Builders<Movie>.Update;

            var updateDefinition = updateBuilder.Set(movie => movie.Id, id);

            if (updatedMovie.name != null)
                updateDefinition = updateDefinition.Set(movie => movie.name, updatedMovie.name);

            if (updatedMovie.studio != null)
                updateDefinition = updateDefinition.Set(movie => movie.studio, updatedMovie.studio);

            if (updatedMovie.publishDate != default)
                updateDefinition = updateDefinition.Set(movie => movie.publishDate, updatedMovie.publishDate);

            if (updatedMovie.genre != null)
                updateDefinition = updateDefinition.Set(movie => movie.genre, updatedMovie.genre);

            if (updatedMovie.type != null)
                updateDefinition = updateDefinition.Set(movie => movie.type, updatedMovie.type);

            if (updatedMovie.actors != null)
                updateDefinition = updateDefinition.Set(movie => movie.actors, updatedMovie.actors);

            if (updatedMovie.director != null)
                updateDefinition = updateDefinition.Set(movie => movie.director, updatedMovie.director);

            if (updatedMovie.description != null)
                updateDefinition = updateDefinition.Set(movie => movie.description, updatedMovie.description);

            if (updatedMovie.image != null)
                updateDefinition = updateDefinition.Set(movie => movie.image, updatedMovie.image);

            if (updatedMovie.trailer != null)
                updateDefinition = updateDefinition.Set(movie => movie.trailer, updatedMovie.trailer);

            if (updatedMovie.duration != 0)
                updateDefinition = updateDefinition.Set(movie => movie.duration, updatedMovie.duration);

            await _movieCollection.UpdateOneAsync(filter, updateDefinition);

        }


        public async Task DeleteAsync(string id)
        {
            FilterDefinition<Movie> filter = Builders<Movie>.Filter.Eq("Id", id);
            await _movieCollection.DeleteOneAsync(filter);
            return;
        }
    }
}
