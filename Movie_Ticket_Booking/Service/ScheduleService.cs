using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Movie_Ticket_Booking.Models;
using System.Text.RegularExpressions;

namespace Movie_Ticket_Booking.Service
{
    public class ScheduleService
    {
        private readonly IMongoCollection<Schedule> _scheduleCollection;

        public ScheduleService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _scheduleCollection = database.GetCollection<Schedule>("schedule");
        }

       /* public async Task<List<Schedule>> GetAsync()
        {
            return await _scheduleCollection.Find(new BsonDocument()).ToListAsync();
        }*/
        public async Task<PagedResult<ScheduleFullinfo>> GetAsync(int page = 1, int pageSize = 10)
        {
            var pipeline = new BsonDocument[]
                    {
                // Perform a left outer join with the "movie" collection
                new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "movie" },
                        { "localField", "movie" },  // Assuming "movieId" is the field linking the two collections
                        { "foreignField", "_id" },
                        { "as", "movie" }
                    }
                ),
                new BsonDocument("$unwind", "$movie"),
                new BsonDocument("$lookup",
                new BsonDocument
                {
                    { "from", "genre" },
                    { "localField", "movie.genre" },  // Assuming "movie.genre" is the field linking the two collections
                    { "foreignField", "_id" },
                    { "as", "movie.genre" }
                }
            ),
                // Perform a left outer join with the "theatre" collection
                new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "theatre" },
                        { "localField", "theatre" },  // Assuming "theatreId" is the field linking the two collections
                        { "foreignField", "_id" },
                        { "as", "theatre" }
                    }
                ),
                new BsonDocument("$unwind", "$theatre"),
                new BsonDocument("$match",
                    new BsonDocument("$expr",
                        new BsonDocument("eq", new BsonArray { new BsonDocument("$size", "$bookedSeat"), 0 })
                    )
                ),
                 new BsonDocument("$lookup",
                new BsonDocument
                {
                    { "from", "ticket" },
                    { "let", new BsonDocument("ticketID", "$bookedSeat") },
                    { "pipeline", new BsonArray
                        {
                            new BsonDocument("$match",
                                new BsonDocument("$expr",
                                    new BsonDocument("$in", new BsonArray { "$_id", "$$ticketID" })
                                )
                            )
                        }
                    },
                    { "as", "bookedSeat" }
                }
            ),

                 new BsonDocument("$unwind",
                    new BsonDocument
                    {
                        { "path", "$bookedSeat" },
                        { "preserveNullAndEmptyArrays", true } 
                    }
                ),
                 new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "seat" },
                        { "localField", "bookedSeat.seat" },
                        { "foreignField", "_id" },
                        { "as", "bookedSeat.seat" }
                    }
                ),
                new BsonDocument("$unwind",
                    new BsonDocument
                    {
                        { "path", "$bookedSeat.seat" },
                        { "preserveNullAndEmptyArrays", true }
                    }
                ),
                new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "voucher" },
                        { "let", new BsonDocument("voucherCode", "$bookedSeat.voucher") },
                        { "pipeline", new BsonArray
                            {
                                new BsonDocument("$match",
                                    new BsonDocument("$expr",
                                        new BsonDocument("$eq", new BsonArray { "$code", "$$voucherCode" })
                                    )
                                )
                            }
                        },
                        { "as", "bookedSeat.voucher" }
                    }
                ),
                new BsonDocument("$unwind", new BsonDocument
                    {
                        { "path", "$bookedSeat.voucher" },
                        { "preserveNullAndEmptyArrays", true }
                    }
                ),

        new BsonDocument("$project",
            new BsonDocument
            {
                { "_id", 1 },
                { "startTime", 1 },
                { "endTime", 1 },
                { "movie._id", 1 },
                { "movie.name", 1 },
                { "movie.studio", 1 },
                { "movie.publishDate", 1 },
                { "movie.endDate", 1 },
                { "movie.genre._id", 1 },
                { "movie.genre.name", 1 },
                { "movie.type", 1 },
                { "movie.actors", 1 },
                { "movie.director", 1 },
                { "movie.description", 1 },
                { "movie.image", 1 },
                { "movie.trailer", 1 },
                { "movie.duration", 1 },
                { "movie.profit", 1 },
                { "theatre._id", 1 },
                { "theatre.name", 1 },
                { "theatre.description", 1 },
                { "price", 1 },
                { "total", 1 },
                { "bookedSeat._id", 1 },
                { "bookedSeat.schedule", 1 },
                { "bookedSeat.seat._id", 1 },
                { "bookedSeat.seat.theatre", 1 },
                { "bookedSeat.seat.row", 1 },
                { "bookedSeat.seat.number", 1 },
                { "bookedSeat.voucher._id", 1 },
                { "bookedSeat.voucher.name", 1 },
                { "bookedSeat.voucher.code", 1 },
                { "bookedSeat.voucher.description", 1 },
                { "bookedSeat.voucher.value", 1 },
                { "bookedSeat.createdAt", 1 },
                { "bookedSeat.updatedAt", 1 },
                { "bookedSeat.price", 1 },

            }
        ),
        new BsonDocument("$group",
            new BsonDocument
            {
                { "_id", "$_id" },
                { "startTime", new BsonDocument("$first", "$startTime") },
                { "endTime", new BsonDocument("$first", "$endTime") },
                { "movie", new BsonDocument("$first", "$movie") },
                { "theatre", new BsonDocument("$first", "$theatre") },
                { "price", new BsonDocument("$first", "$price") },
                { "total", new BsonDocument("$first", "$total") },
                { "bookedSeat", new BsonDocument("$push", "$bookedSeat") },
            }
        ),

        new BsonDocument("$sort",
            new BsonDocument
            {
                { "startTime", 1 }
            }
        ),
        new BsonDocument("$skip", (page - 1) * pageSize),
                     new BsonDocument("$limit", pageSize),
            };

            var totalSeats = await _scheduleCollection.CountDocumentsAsync(new BsonDocument());

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _scheduleCollection.Aggregate<ScheduleFullinfo>(pipeline, options).ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalSeats / pageSize);

            var pagedResult = new PagedResult<ScheduleFullinfo>
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Data = result
            };

            return pagedResult;
        }

        public async Task<PagedResult<ScheduleFullinfo>> SearchAsync(string query, int page = 1, int pageSize = 10)
        {
            var matchStage = new BsonDocument("$match",
                new BsonDocument
                {
            {
                "$or", new BsonArray
                {
                    new BsonDocument("movie.name", new BsonDocument("$regex", new BsonRegularExpression(query, "i"))),
                    new BsonDocument("theatre.name", new BsonDocument("$regex", new BsonRegularExpression(query, "i")))
                }
            }
                });

            var pipeline = new BsonDocument[]
            {
        // Perform a left outer join with the "movie" collection
        new BsonDocument("$lookup",
            new BsonDocument
            {
                { "from", "movie" },
                { "localField", "movie" },
                { "foreignField", "_id" },
                { "as", "movie" }
            }
        ),
        new BsonDocument("$unwind", "$movie"),
        new BsonDocument("$lookup",
            new BsonDocument
            {
                { "from", "genre" },
                { "localField", "movie.genre" },
                { "foreignField", "_id" },
                { "as", "movie.genre" }
            }
        ),
        // Perform a left outer join with the "theatre" collection
        new BsonDocument("$lookup",
            new BsonDocument
            {
                { "from", "theatre" },
                { "localField", "theatre" },
                { "foreignField", "_id" },
                { "as", "theatre" }
            }
        ),
        new BsonDocument("$unwind", "$theatre"),
        matchStage, // Add the match stage for search

        new BsonDocument("$match",
                    new BsonDocument("$expr",
                        new BsonDocument("eq", new BsonArray { new BsonDocument("$size", "$bookedSeat"), 0 })
                    )
                ),
                 new BsonDocument("$lookup",
                new BsonDocument
                {
                    { "from", "ticket" },
                    { "let", new BsonDocument("ticketID", "$bookedSeat") },
                    { "pipeline", new BsonArray
                        {
                            new BsonDocument("$match",
                                new BsonDocument("$expr",
                                    new BsonDocument("$in", new BsonArray { "$_id", "$$ticketID" })
                                )
                            )
                        }
                    },
                    { "as", "bookedSeat" }
                }
            ),

                 new BsonDocument("$unwind",
                    new BsonDocument
                    {
                        { "path", "$bookedSeat" },
                        { "preserveNullAndEmptyArrays", true }
                    }
                ),
                 new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "seat" },
                        { "localField", "bookedSeat.seat" },
                        { "foreignField", "_id" },
                        { "as", "bookedSeat.seat" }
                    }
                ),
                new BsonDocument("$unwind",
                    new BsonDocument
                    {
                        { "path", "$bookedSeat.seat" },
                        { "preserveNullAndEmptyArrays", true }
                    }
                ),
                new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "voucher" },
                        { "let", new BsonDocument("voucherCode", "$bookedSeat.voucher") },
                        { "pipeline", new BsonArray
                            {
                                new BsonDocument("$match",
                                    new BsonDocument("$expr",
                                        new BsonDocument("$eq", new BsonArray { "$code", "$$voucherCode" })
                                    )
                                )
                            }
                        },
                        { "as", "bookedSeat.voucher" }
                    }
                ),
                new BsonDocument("$unwind", new BsonDocument
                    {
                        { "path", "$bookedSeat.voucher" },
                        { "preserveNullAndEmptyArrays", true }
                    }
                ),

        new BsonDocument("$project",
            new BsonDocument
            {
                { "_id", 1 },
                { "startTime", 1 },
                { "endTime", 1 },
                { "movie._id", 1 },
                { "movie.name", 1 },
                { "movie.studio", 1 },
                { "movie.publishDate", 1 },
                { "movie.endDate", 1 },
                { "movie.genre._id", 1 },
                { "movie.genre.name", 1 },
                { "movie.type", 1 },
                { "movie.actors", 1 },
                { "movie.director", 1 },
                { "movie.description", 1 },
                { "movie.image", 1 },
                { "movie.trailer", 1 },
                { "movie.duration", 1 },
                { "movie.profit", 1 },
                { "theatre._id", 1 },
                { "theatre.name", 1 },
                { "theatre.description", 1 },
                { "price", 1 },
                { "total", 1 },
                { "bookedSeat._id", 1 },
                { "bookedSeat.schedule", 1 },
                { "bookedSeat.seat._id", 1 },
                { "bookedSeat.seat.theatre", 1 },
                { "bookedSeat.seat.row", 1 },
                { "bookedSeat.seat.number", 1 },
                { "bookedSeat.voucher._id", 1 },
                { "bookedSeat.voucher.name", 1 },
                { "bookedSeat.voucher.code", 1 },
                { "bookedSeat.voucher.description", 1 },
                { "bookedSeat.voucher.value", 1 },
                { "bookedSeat.createdAt", 1 },
                { "bookedSeat.updatedAt", 1 },
                { "bookedSeat.price", 1 },

            }
        ),
        new BsonDocument("$group",
            new BsonDocument
            {
                { "_id", "$_id" },
                { "startTime", new BsonDocument("$first", "$startTime") },
                { "endTime", new BsonDocument("$first", "$endTime") },
                { "movie", new BsonDocument("$first", "$movie") },
                { "theatre", new BsonDocument("$first", "$theatre") },
                { "price", new BsonDocument("$first", "$price") },
                { "total", new BsonDocument("$first", "$total") },
                { "bookedSeat", new BsonDocument("$push", "$bookedSeat") },
            }
        ),
        new BsonDocument("$sort",
            new BsonDocument
            {
                { "startTime", 1 }
            }
        ),
        new BsonDocument("$skip", (page - 1) * pageSize),
        new BsonDocument("$limit", pageSize),
            };

            var totalSeats = await _scheduleCollection.CountDocumentsAsync(new BsonDocument());

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _scheduleCollection.Aggregate<ScheduleFullinfo>(pipeline, options).ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalSeats / pageSize);

            var pagedResult = new PagedResult<ScheduleFullinfo>
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Data = result
            };

            return pagedResult;
        }

        public async Task CreateAsync(Schedule schedule)
        {
            // Đảm bảo định dạng ngày và giờ là UTC trước khi lưu trữ
            schedule.startTime = DateTime.SpecifyKind(schedule.startTime, DateTimeKind.Utc);
            schedule.endTime = DateTime.SpecifyKind(schedule.endTime, DateTimeKind.Utc);
            await _scheduleCollection.InsertOneAsync(schedule);
            return;
        }

        public async Task<List<ScheduleFullinfo>> GetByIdAsync(string id)
        {
            var pipeline = new BsonDocument[]
                    {
                         new BsonDocument("$match",
                        new BsonDocument
                        {

                            { "_id", new BsonObjectId(ObjectId.Parse(id)) }
                        }
                    ),
                // Perform a left outer join with the "movie" collection
                new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "movie" },
                        { "localField", "movie" },  // Assuming "movieId" is the field linking the two collections
                        { "foreignField", "_id" },
                        { "as", "movie" }
                    }
                ),
                new BsonDocument("$unwind", "$movie"),
                new BsonDocument("$lookup",
                new BsonDocument
                {
                    { "from", "genre" },
                    { "localField", "movie.genre" },  // Assuming "movie.genre" is the field linking the two collections
                    { "foreignField", "_id" },
                    { "as", "movie.genre" }
                }
            ),
                // Perform a left outer join with the "theatre" collection
                new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "theatre" },
                        { "localField", "theatre" },  // Assuming "theatreId" is the field linking the two collections
                        { "foreignField", "_id" },
                        { "as", "theatre" }
                    }
                ),
                new BsonDocument("$unwind", "$theatre"),
                 new BsonDocument("$match",
                    new BsonDocument("$expr",
                        new BsonDocument("eq", new BsonArray { new BsonDocument("$size", "$bookedSeat"), 0 })
                    )
                ),
                 new BsonDocument("$lookup",
                new BsonDocument
                {
                    { "from", "ticket" },
                    { "let", new BsonDocument("ticketID", "$bookedSeat") },
                    { "pipeline", new BsonArray
                        {
                            new BsonDocument("$match",
                                new BsonDocument("$expr",
                                    new BsonDocument("$in", new BsonArray { "$_id", "$$ticketID" })
                                )
                            )
                        }
                    },
                    { "as", "bookedSeat" }
                }
            ),

                 new BsonDocument("$unwind",
                    new BsonDocument
                    {
                        { "path", "$bookedSeat" },
                        { "preserveNullAndEmptyArrays", true }
                    }
                ),
                 new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "seat" },
                        { "localField", "bookedSeat.seat" },
                        { "foreignField", "_id" },
                        { "as", "bookedSeat.seat" }
                    }
                ),
                new BsonDocument("$unwind",
                    new BsonDocument
                    {
                        { "path", "$bookedSeat.seat" },
                        { "preserveNullAndEmptyArrays", true }
                    }
                ),
                new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "voucher" },
                        { "let", new BsonDocument("voucherCode", "$bookedSeat.voucher") },
                        { "pipeline", new BsonArray
                            {
                                new BsonDocument("$match",
                                    new BsonDocument("$expr",
                                        new BsonDocument("$eq", new BsonArray { "$code", "$$voucherCode" })
                                    )
                                )
                            }
                        },
                        { "as", "bookedSeat.voucher" }
                    }
                ),
                new BsonDocument("$unwind", new BsonDocument
                    {
                        { "path", "$bookedSeat.voucher" },
                        { "preserveNullAndEmptyArrays", true }
                    }
                ),

        new BsonDocument("$project",
            new BsonDocument
            {
                { "_id", 1 },
                { "startTime", 1 },
                { "endTime", 1 },
                { "movie._id", 1 },
                { "movie.name", 1 },
                { "movie.studio", 1 },
                { "movie.publishDate", 1 },
                { "movie.endDate", 1 },
                { "movie.genre._id", 1 },
                { "movie.genre.name", 1 },
                { "movie.type", 1 },
                { "movie.actors", 1 },
                { "movie.director", 1 },
                { "movie.description", 1 },
                { "movie.image", 1 },
                { "movie.trailer", 1 },
                { "movie.duration", 1 },
                { "movie.profit", 1 },
                { "theatre._id", 1 },
                { "theatre.name", 1 },
                { "theatre.description", 1 },
                { "price", 1 },
                { "total", 1 },
                { "bookedSeat._id", 1 },
                { "bookedSeat.schedule", 1 },
                { "bookedSeat.seat._id", 1 },
                { "bookedSeat.seat.theatre", 1 },
                { "bookedSeat.seat.row", 1 },
                { "bookedSeat.seat.number", 1 },
                { "bookedSeat.voucher._id", 1 },
                { "bookedSeat.voucher.name", 1 },
                { "bookedSeat.voucher.code", 1 },
                { "bookedSeat.voucher.description", 1 },
                { "bookedSeat.voucher.value", 1 },
                { "bookedSeat.createdAt", 1 },
                { "bookedSeat.updatedAt", 1 },
                { "bookedSeat.price", 1 },
                
            }
        ),
        new BsonDocument("$group",
            new BsonDocument
            {
                { "_id", "$_id" },
                { "startTime", new BsonDocument("$first", "$startTime") },
                { "endTime", new BsonDocument("$first", "$endTime") },
                { "movie", new BsonDocument("$first", "$movie") },
                { "theatre", new BsonDocument("$first", "$theatre") },
                { "price", new BsonDocument("$first", "$price") },
                { "total", new BsonDocument("$first", "$total") },
                { "bookedSeat", new BsonDocument("$push", "$bookedSeat") },
            }
        ),
        new BsonDocument("$sort",
            new BsonDocument
            {
                { "startTime", 1 }
            }
        ),

            };

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _scheduleCollection.Aggregate<ScheduleFullinfo>(pipeline, options).ToListAsync();
            return result;


        }

        public async Task<(bool success, string errorMessage)> UpdateAsync(string id, Schedule updatedSchedule)
        {
            var filter = Builders<Schedule>.Filter.Eq(schedule => schedule.Id, id);
            var existingSchedule = await _scheduleCollection.Find(filter).FirstOrDefaultAsync();
            var updateBuilder = Builders<Schedule>.Update;

            if (existingSchedule == null)
            {
                return (false, "Schedule not found");
            }


            // Check if bookedSeat has any elements before allowing the update
            if (existingSchedule.bookedSeat == null || !existingSchedule.bookedSeat.Any())
            {
                var updateDefinition = updateBuilder.Set(schedule => schedule.Id, id);

                if (updatedSchedule.startTime != default)
                    updateDefinition = updateDefinition.Set(schedule => schedule.startTime, updatedSchedule.startTime);

                if (updatedSchedule.endTime != default)
                    updateDefinition = updateDefinition.Set(schedule => schedule.endTime, updatedSchedule.endTime);

                if (updatedSchedule.movie != null)
                    updateDefinition = updateDefinition.Set(schedule => schedule.movie, updatedSchedule.movie);

                if (updatedSchedule.theatre != null)
                    updateDefinition = updateDefinition.Set(schedule => schedule.theatre, updatedSchedule.theatre);

                if (updatedSchedule.price != default)
                    updateDefinition = updateDefinition.Set(schedule => schedule.price, updatedSchedule.price);

                if (updatedSchedule.bookedSeat != null)
                    updateDefinition = updateDefinition.Set(schedule => schedule.bookedSeat, updatedSchedule.bookedSeat);

                var result = await _scheduleCollection.UpdateOneAsync(filter, updateDefinition);

                return (result.ModifiedCount > 0, "Schedule is updated" );
            }
            else
            {
                return (false, "Cannot update schedule with booked seats.");
            }
        }


        public async Task<List<ScheduleFullinfo>> GetByMovieIdAsync(string movieId)
        {
            var pipeline = new BsonDocument[]
                    {
                     new BsonDocument("$match",
                        new BsonDocument
                        {
                            { "movie", new BsonObjectId(ObjectId.Parse(movieId)) }
                        }
                    ),
                // Perform a left outer join with the "movie" collection
                new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "movie" },
                        { "localField", "movie" },  // Assuming "movieId" is the field linking the two collections
                        { "foreignField", "_id" },
                        { "as", "movie" }
                    }
                ),
                new BsonDocument("$unwind", "$movie"),
                new BsonDocument("$lookup",
                new BsonDocument
                {
                    { "from", "genre" },
                    { "localField", "movie.genre" },  // Assuming "movie.genre" is the field linking the two collections
                    { "foreignField", "_id" },
                    { "as", "movie.genre" }
                }
            ),
                // Perform a left outer join with the "theatre" collection
                new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "theatre" },
                        { "localField", "theatre" },  // Assuming "theatreId" is the field linking the two collections
                        { "foreignField", "_id" },
                        { "as", "theatre" }
                    }
                ),
                new BsonDocument("$unwind", "$theatre"),
                new BsonDocument("$match",
                    new BsonDocument("$expr",
                        new BsonDocument("eq", new BsonArray { new BsonDocument("$size", "$bookedSeat"), 0 })
                    )
                ),
                 new BsonDocument("$lookup",
                new BsonDocument
                {
                    { "from", "ticket" },
                    { "let", new BsonDocument("ticketID", "$bookedSeat") },
                    { "pipeline", new BsonArray
                        {
                            new BsonDocument("$match",
                                new BsonDocument("$expr",
                                    new BsonDocument("$in", new BsonArray { "$_id", "$$ticketID" })
                                )
                            )
                        }
                    },
                    { "as", "bookedSeat" }
                }
            ),

                 new BsonDocument("$unwind",
                    new BsonDocument
                    {
                        { "path", "$bookedSeat" },
                        { "preserveNullAndEmptyArrays", true }
                    }
                ),
                 new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "seat" },
                        { "localField", "bookedSeat.seat" },
                        { "foreignField", "_id" },
                        { "as", "bookedSeat.seat" }
                    }
                ),
                new BsonDocument("$unwind",
                    new BsonDocument
                    {
                        { "path", "$bookedSeat.seat" },
                        { "preserveNullAndEmptyArrays", true }
                    }
                ),
                new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "voucher" },
                        { "let", new BsonDocument("voucherCode", "$bookedSeat.voucher") },
                        { "pipeline", new BsonArray
                            {
                                new BsonDocument("$match",
                                    new BsonDocument("$expr",
                                        new BsonDocument("$eq", new BsonArray { "$code", "$$voucherCode" })
                                    )
                                )
                            }
                        },
                        { "as", "bookedSeat.voucher" }
                    }
                ),
                new BsonDocument("$unwind", new BsonDocument
                    {
                        { "path", "$bookedSeat.voucher" },
                        { "preserveNullAndEmptyArrays", true }
                    }
                ),

        new BsonDocument("$project",
            new BsonDocument
            {
                { "_id", 1 },
                { "startTime", 1 },
                { "endTime", 1 },
                { "movie._id", 1 },
                { "movie.name", 1 },
                { "movie.studio", 1 },
                { "movie.publishDate", 1 },
                { "movie.endDate", 1 },

                { "movie.genre._id", 1 },
                { "movie.genre.name", 1 },
                { "movie.type", 1 },
                { "movie.actors", 1 },
                { "movie.director", 1 },
                { "movie.description", 1 },
                { "movie.image", 1 },
                { "movie.trailer", 1 },
                { "movie.duration", 1 },
                { "movie.profit", 1 },
                { "theatre._id", 1 },
                { "theatre.name", 1 },
                { "theatre.description", 1 },
                { "price", 1 },
                { "total", 1 },
                { "bookedSeat._id", 1 },
                { "bookedSeat.schedule", 1 },
                { "bookedSeat.seat._id", 1 },
                { "bookedSeat.seat.theatre", 1 },
                { "bookedSeat.seat.row", 1 },
                { "bookedSeat.seat.number", 1 },
                { "bookedSeat.voucher._id", 1 },
                { "bookedSeat.voucher.name", 1 },
                { "bookedSeat.voucher.code", 1 },
                { "bookedSeat.voucher.description", 1 },
                { "bookedSeat.voucher.value", 1 },
                { "bookedSeat.createdAt", 1 },
                { "bookedSeat.updatedAt", 1 },
                { "bookedSeat.price", 1 },

            }
        ),
        new BsonDocument("$group",
            new BsonDocument
            {
                { "_id", "$_id" },
                { "startTime", new BsonDocument("$first", "$startTime") },
                { "endTime", new BsonDocument("$first", "$endTime") },
                { "movie", new BsonDocument("$first", "$movie") },
                { "theatre", new BsonDocument("$first", "$theatre") },
                { "price", new BsonDocument("$first", "$price") },
                { "total", new BsonDocument("$first", "$total") },
                { "bookedSeat", new BsonDocument("$push", "$bookedSeat") },
            }
        ),
        new BsonDocument("$sort",
            new BsonDocument
            {
                { "startTime", 1 }
            }
        ),
            };

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _scheduleCollection.Aggregate<ScheduleFullinfo>(pipeline, options).ToListAsync();
            return result;


        }

        public async Task DeleteAsync(string id)
        {
            FilterDefinition<Schedule> filter = Builders<Schedule>.Filter.Eq("Id", id);
            await _scheduleCollection.DeleteOneAsync(filter);
            return;
        }
    }
}

