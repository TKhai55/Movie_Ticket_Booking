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

        public async Task<List<NewsWithCreator>> GetAsync()
        {
            var pipeline = new BsonDocument[]
               {
                 new BsonDocument("$lookup",
                     new BsonDocument
                     {
                         { "from", "account" },
                         { "localField", "creator" },
                         { "foreignField", "_id" },
                         { "as", "creatorInfo" }
                     }
                  ),
                 new BsonDocument("$unwind", "$creatorInfo"),
                 new BsonDocument("$project",
                     new BsonDocument
                     {
                         { "_id", 1 },
                         { "title", 1 },
                         { "content", 1 },
                         { "createdAt", 1 },
                         { "creatorInfo._id", 1 },
                         { "creatorInfo.account", 1 },
                         { "creatorInfo.password", 1 },
                     }),
                };

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _newsCollection.Aggregate<NewsWithCreator>(pipeline, options).ToListAsync();
            return result;
        }

        public async Task<List<NewsWithCreator>> GetByIdAsync(string id)
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
                         { "from", "account" },
                         { "localField", "creator" },
                         { "foreignField", "_id" },
                         { "as", "creatorInfo" }
                     }
                  ),
                 new BsonDocument("$unwind", "$creatorInfo"),
                 new BsonDocument("$project",
                     new BsonDocument
                     {
                         { "_id", 1 },
                         { "title", 1 },
                         { "content", 1 },
                         { "createdAt", 1 },
                         { "creatorInfo._id", 1 },
                         { "creatorInfo.account", 1 },
                         { "creatorInfo.password", 1 },
                     }),
                };

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _newsCollection.Aggregate<NewsWithCreator>(pipeline, options).ToListAsync();
            return result;
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
