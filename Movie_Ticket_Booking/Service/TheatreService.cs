using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Movie_Ticket_Booking.Models;

namespace Movie_Ticket_Booking.Service
{
    public class TheatreService
    {
        private readonly IMongoCollection<Theatre> _theatreCollection;
        private readonly IMongoCollection<Seat> _seatCollection;

        public TheatreService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _theatreCollection = database.GetCollection<Theatre>("theatre");
            _seatCollection = database.GetCollection<Seat>("seat");
        }

        public async Task<PagedResult<Theatre>> GetAsync(int page = 1, int pageSize = 10)
        {
            var totalVouchers = await _theatreCollection.CountDocumentsAsync(new BsonDocument());

            var pipeline = new BsonDocument[]
            {
            };

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _theatreCollection.Aggregate<Theatre>(pipeline, options).ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalVouchers / pageSize);

            var pagedResult = new PagedResult<Theatre>
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Data = result
            };

            return pagedResult;
        }
        public async Task CreateAsync(Theatre theatre)
        {
            await _theatreCollection.InsertOneAsync(theatre);
            // Tạo 100 tài liệu Seat tương ứng với Theatre đã tạo
            for (char row = 'A'; row <= 'J'; row++)
            {
                for (int number = 1; number <= 10; number++)
                {
                    var seat = new Seat
                    {
                        theatre = theatre.Id, // Sử dụng Id của Theatre đã tạo
                        row = row.ToString(),
                        number = number
                    };
                    await _seatCollection.InsertOneAsync(seat);
                }
            }
            return;
        }

        public async Task<Theatre> GetByIdAsync(string id)
        {
            var theatre = await _theatreCollection.Find(t => t.Id == id).FirstOrDefaultAsync();
            return theatre;
        }

        public async Task UpdateAsync(string id, Theatre updatedTheatre)
        {
            var filter = Builders<Theatre>.Filter.Eq(movie => movie.Id, id);
            var updateBuilder = Builders<Theatre>.Update;

            var updateDefinition = updateBuilder.Set(movie => movie.Id, id);

            if (updatedTheatre.name != null)
                updateDefinition = updateDefinition.Set(movie => movie.name, updatedTheatre.name);

            if (updatedTheatre.description != null)
                updateDefinition = updateDefinition.Set(movie => movie.description, updatedTheatre.description);


            await _theatreCollection.UpdateOneAsync(filter, updateDefinition);
        }

        public async Task DeleteAsync(string id)
        {
            // Tìm theatre cần xoá
            FilterDefinition<Theatre> filter = Builders<Theatre>.Filter.Eq("Id", id);
            var theatreToDelete = await _theatreCollection.Find(filter).FirstOrDefaultAsync();

            if (theatreToDelete == null)
            {
                return;
            }

            // Xoá theatre
            await _theatreCollection.DeleteOneAsync(filter);

            // Xoá tất cả các seat có theatre bằng với Id của theatre đã xoá
            FilterDefinition<Seat> seatFilter = Builders<Seat>.Filter.Eq("theatre", theatreToDelete.Id);
            await _seatCollection.DeleteManyAsync(seatFilter);
        }

    }
}
