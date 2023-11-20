using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Movie_Ticket_Booking.Models;

namespace Movie_Ticket_Booking.Service
{
    public class TicketService
    {
        private readonly IMongoCollection<Ticket> _ticketCollection;
        private readonly IMongoCollection<Movie> _movieCollection;
        private readonly IMongoCollection<Schedule> _scheduleCollection;

        public TicketService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _ticketCollection = database.GetCollection<Ticket>("ticket");
            _movieCollection = database.GetCollection<Movie>("movie");
            _scheduleCollection = database.GetCollection<Schedule>("schedule");
        }

        public async Task<List<TicketInformation>> GetAsync()
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "seat" },
                        { "localField", "seat" },
                        { "foreignField", "_id" },
                        { "as", "seat" }
                    }
                ),
                new BsonDocument("$unwind", "$seat"),
                new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "voucher" },
                        { "let", new BsonDocument("voucherId", "$voucher") },
                        { "pipeline", new BsonArray
                            {
                                new BsonDocument("$match",
                                    new BsonDocument("$expr",
                                        new BsonDocument("$eq", new BsonArray { "$_id", "$$voucherId" })
                                    )
                                )
                            }
                        },
                        { "as", "voucher" }
                    }
                ),
                new BsonDocument("$unwind", new BsonDocument
                    {
                        { "path", "$voucher" },
                        { "preserveNullAndEmptyArrays", true }
                    }
                ),
                new BsonDocument("$project",
                    new BsonDocument
                    {
                        { "_id", 1 },
                        { "schedule", 1 },
                        { "voucher._id", 1 },
                        { "voucher.name", 1 },
                        { "voucher.value", 1 },
                        { "voucher.description", 1 },
                        { "seat._id", 1 },
                        { "seat.theatre", 1 },
                        { "seat.row", 1 },
                        { "seat.number", 1 },
                        { "createdAt", 1 },
                        { "updatedAt", 1 },
                        { "price", 1 },
                    }
                ),
                new BsonDocument("$sort",
                    new BsonDocument
                    {
                        { "updatedAt", -1 }
                    }
                ),
            };

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _ticketCollection.Aggregate<TicketInformation>(pipeline, options).ToListAsync();
            return result;
        }
        public async Task CreateAsync(Ticket ticket)
        {
            await _ticketCollection.InsertOneAsync(ticket);

            // Gọi hàm cập nhật revenue và total
            await UpdateRevenueAndTotal(ticket);

            await UpdateBookedSeats(ticket);
            return;
        }

        private async Task UpdateBookedSeats(Ticket ticket)
        {
            var scheduleId = ticket.schedule;

            var filter = Builders<Schedule>.Filter.Eq(x => x.Id, scheduleId);
            var update = Builders<Schedule>.Update.Push(x => x.bookedSeat, ticket.Id);

            await _scheduleCollection.UpdateOneAsync(filter, update);
        }
        private async Task UpdateRevenueAndTotal(Ticket ticket)
        {
            var scheduleId = ticket.schedule;
            var schedule = await _scheduleCollection.Find(x => x.Id == scheduleId).FirstOrDefaultAsync();

            if (schedule != null)
            {
                var movieId = schedule.movie;
                var movie = await _movieCollection.Find(x => x.Id == movieId).FirstOrDefaultAsync();

                if (movie != null)
                {
                    // Cộng giá trị price vào revenue của movie
                    movie.profit += ticket.price;

                    // Cộng giá trị price vào total của schedule
                    schedule.total += ticket.price;

                    // Cập nhật thông tin movie và schedule sau khi cộng giá trị
                    var movieUpdate = Builders<Movie>.Update.Set(x => x.profit, movie.profit);
                    await _movieCollection.UpdateOneAsync(x => x.Id == movie.Id, movieUpdate);

                    var scheduleUpdate = Builders<Schedule>.Update.Set(x => x.total, schedule.total);
                    await _scheduleCollection.UpdateOneAsync(x => x.Id == schedule.Id, scheduleUpdate);
                }
            }
        }

        public async Task<List<TicketInformation>> GetByIdAsync(string id)
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
                         { "from", "seat" },
                         { "localField", "seat" },
                         { "foreignField", "_id" },
                         { "as", "seat" }
                     }

                  ),
                 new BsonDocument("$unwind", "$seat"),
                 new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "voucher" },
                        { "let", new BsonDocument("voucherId", "$voucher") },
                        { "pipeline", new BsonArray
                            {
                                new BsonDocument("$match",
                                    new BsonDocument("$expr",
                                        new BsonDocument("$eq", new BsonArray { "$_id", "$$voucherId" })
                                    )
                                )
                            }
                        },
                        { "as", "voucher" }
                    }
                ),
                new BsonDocument("$unwind", new BsonDocument
                    {
                        { "path", "$voucher" },
                        { "preserveNullAndEmptyArrays", true }
                    }
                ),
                 new BsonDocument("$project",
                     new BsonDocument
                     {
                         { "_id", 1 },
                         { "schedule", 1 },
                         { "voucher._id", 1 },
                         { "voucher.name", 1 },
                         { "voucher.value", 1 },
                         { "voucher.description", 1 },

                         { "seat._id", 1 },
                         { "seat.theatre", 1 },
                         { "seat.row", 1 },
                         { "seat.number", 1 },
                         { "createdAt", 1 },
                         { "updatedAt", 1 },
                         { "price", 1 },
                     }),
             };

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _ticketCollection.Aggregate<TicketInformation>(pipeline, options).ToListAsync();
            return result;
        }
        public async Task DeleteAsync(string id)
        {
            FilterDefinition<Ticket> filter = Builders<Ticket>.Filter.Eq("Id", id);
            await _ticketCollection.DeleteOneAsync(filter);
            return;
        }
    }
}
