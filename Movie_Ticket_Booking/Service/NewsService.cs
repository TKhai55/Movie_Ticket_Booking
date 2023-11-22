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

        public async Task<PagedResult<NewsWithCreator>> GetAsync(int page = 1, int pageSize = 10)
        {
            var pipeline = new BsonDocument[]
               {
                 new BsonDocument("$lookup",
                     new BsonDocument
                     {
                         { "from", "account" },
                         { "localField", "creator" },
                         { "foreignField", "_id" },
                         { "as", "creator" }
                     }
                  ),
                 new BsonDocument("$unwind", "$creator"),
                 new BsonDocument("$project",
                     new BsonDocument
                     {
                         { "_id", 1 },
                         { "title", 1 },
                         { "content", 1 },
                         { "createdAt", 1 },
                         { "updatedAt", 1 },
                         { "creator._id", 1 },
                         { "creator.account", 1 },
                     }),
                     new BsonDocument("$sort",
                        new BsonDocument
                        {
                            { "updatedAt", -1 }
                        }
                    ),
              new BsonDocument("$skip", (page - 1) * pageSize),
                     new BsonDocument("$limit", pageSize),
             };

            var totalSeats = await _newsCollection.CountDocumentsAsync(new BsonDocument());

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _newsCollection.Aggregate<NewsWithCreator>(pipeline, options).ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalSeats / pageSize);

            var pagedResult = new PagedResult<NewsWithCreator>
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Data = result
            };

            return pagedResult;
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
                         { "as", "creator" }
                     }
                  ),
                 new BsonDocument("$unwind", "$creator"),
                 new BsonDocument("$project",
                     new BsonDocument
                     {
                         { "_id", 1 },
                         { "title", 1 },
                         { "content", 1 },
                         { "createdAt", 1 },
                         { "updatedAt", 1 },
                         { "creator._id", 1 },
                         { "creator.account", 1 },
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

        public async Task UpdateAsync(string id, News updatedNews)
        {
            var filter = Builders<News>.Filter.Eq("_id", new ObjectId(id)); // Lọc dựa trên ID
            var update = Builders<News>.Update
                .Set("updatedAt", DateTime.UtcNow); // Tự động cập nhật trường updatedAt

            if (updatedNews != null)
            {
                // Cập nhật trường title nếu có giá trị
                if (!string.IsNullOrEmpty(updatedNews.title))
                {
                    update = update.Set("title", updatedNews.title);
                }

                // Cập nhật trường content nếu có giá trị
                if (!string.IsNullOrEmpty(updatedNews.content))
                {
                    update = update.Set("content", updatedNews.content);
                }
            }

            // Thực hiện cập nhật
            var result = await _newsCollection.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0)
            {
                // Không tìm thấy tài liệu cần cập nhật
                throw new Exception("News not found");
            }
        }

        public async Task<PagedResult<NewsWithCreator>> SearchAsync(string query, int page = 1, int pageSize = 10)
        {
            var pipeline = new BsonDocument[]
            {
                // ... existing pipeline stages ...

                new BsonDocument("$match", new BsonDocument("title", new BsonRegularExpression(query, "i"))
                ),
                 new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "account" },
                        { "localField", "creator" },
                        { "foreignField", "_id" },
                        { "as", "creator" }
                    }
                ),
                  new BsonDocument("$unwind", "$creator"),
                new BsonDocument("$project", new BsonDocument
                {
                    { "_id", 1 },
                    { "title", 1 },
                    { "content", 1 },
                    { "createdAt", 1 },
                    { "updatedAt", 1 },
                    { "creator._id", 1 },
                    { "creator.account", 1 },
                }),
                new BsonDocument("$skip", (page - 1) * pageSize),
                new BsonDocument("$limit", pageSize),
            };

            var totalSeats = await _newsCollection.CountDocumentsAsync(new BsonDocument());

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _newsCollection.Aggregate<NewsWithCreator>(pipeline, options).ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalSeats / pageSize);

            var pagedResult = new PagedResult<NewsWithCreator>
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Data = result
            };

            return pagedResult;
        }
        public async Task DeleteAsync(string id)
        {
            FilterDefinition<News> filter = Builders<News>.Filter.Eq("Id", id);
            await _newsCollection.DeleteOneAsync(filter);
            return;
        }
    }
}
