﻿using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Movie_Ticket_Booking.Models;
using System.Threading.Tasks;

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

        public async Task<PagedResult<MovieWithGenre>> GetAsync(int page = 1, int pageSize = 10)
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
                        { "endDate", 1 },
                        { "genre._id", 1 },
                        { "genre.name", 1 },
                        { "type", 1 },
                        { "actors", 1 },
                        { "director", 1 },
                        { "description", 1 },
                        { "image", 1 },
                        { "trailer", 1 },
                        { "duration", 1 },
                        { "profit", 1 },
                    }
                ),
                new BsonDocument("$sort",
                        new BsonDocument
                        {
                            { "publishDate", 1 }
                        }
                    ),
            new BsonDocument("$skip", (page - 1) * pageSize),
                     new BsonDocument("$limit", pageSize),
             };

            var totalSeats = await _movieCollection.CountDocumentsAsync(new BsonDocument());

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _movieCollection.Aggregate<MovieWithGenre>(pipeline, options).ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalSeats / pageSize);

            var pagedResult = new PagedResult<MovieWithGenre>
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Data = result
            };

            return pagedResult;
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
                        { "endDate", 1 },
                        { "genre._id", 1 },
                        { "genre.name", 1 },
                        { "type", 1 },
                        { "actors", 1 },
                        { "director", 1 },
                        { "description", 1 },
                        { "image", 1 },
                        { "trailer", 1 },
                        { "duration", 1 },
                        { "profit", 1 },

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

            if (updatedMovie.endDate != default)
                updateDefinition = updateDefinition.Set(movie => movie.endDate, updatedMovie.endDate);

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

        public async Task<PagedResult<MovieWithGenre>> SearchAsync(string query, int page = 1, int pageSize = 10)
        {
            var pipeline = new BsonDocument[]
            {
                // ... existing pipeline stages ...

                new BsonDocument("$match", new BsonDocument("$or", new BsonArray
                {
                    new BsonDocument("name", new BsonRegularExpression(query, "i")),
                    new BsonDocument("studio", new BsonRegularExpression(query, "i"))
                })),
                new BsonDocument("$project", new BsonDocument
                {
                    { "_id", 1 }, 
                    { "name", 1 },
                    { "studio", 1 },
                    { "publishDate", 1 },
                    { "endDate", 1 },
                    { "genre._id", 1 },
                    { "genre.name", 1 },
                    { "type", 1 },
                    { "actors", 1 },
                    { "director", 1 },
                    { "description", 1 },
                    { "image", 1 },
                    { "trailer", 1 },
                    { "duration", 1 },
                    { "profit", 1 },
                }),
                 new BsonDocument("$skip", (page - 1) * pageSize),
                new BsonDocument("$limit", pageSize),
            };

            var totalSeats = await _movieCollection.CountDocumentsAsync(new BsonDocument());

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _movieCollection.Aggregate<MovieWithGenre>(pipeline, options).ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalSeats / pageSize);

            var pagedResult = new PagedResult<MovieWithGenre>
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Data = result
            };

            return pagedResult;
        }

        public async Task<PagedResult<MovieWithGenre>> SearchByGenreAsync(ObjectId genreId, int page = 1, int pageSize = 10)
        {
            var pipeline = new BsonDocument[]
            {
        // ... existing pipeline stages ...

        new BsonDocument("$match", new BsonDocument
        {
            { "genre", new BsonDocument("$in", new BsonArray { genreId }) }
        }),
         new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", "genre" },
                        { "localField", "genre" },
                        { "foreignField", "_id" },
                        { "as", "genre" }
                    }
                ),
        new BsonDocument("$project", new BsonDocument
        {
            { "_id", 1 },
            { "name", 1 },
            { "studio", 1 },
            { "publishDate", 1 },
            { "endDate", 1 },
            { "genre._id", 1 },
            { "genre.name", 1 },
            { "type", 1 },
            { "actors", 1 },
            { "director", 1 },
            { "description", 1 },
            { "image", 1 },
            { "trailer", 1 },
            { "duration", 1 },
            { "profit", 1 },
        }),
                 new BsonDocument("$skip", (page - 1) * pageSize),
                new BsonDocument("$limit", pageSize),
            };

            var totalSeats = await _movieCollection.CountDocumentsAsync(new BsonDocument());

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _movieCollection.Aggregate<MovieWithGenre>(pipeline, options).ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalSeats / pageSize);

            var pagedResult = new PagedResult<MovieWithGenre>
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Data = result
            };

            return pagedResult;
        }

        public async Task<PagedResult<MovieWithGenre>> GetUpcomingAsync(int page = 1, int pageSize = 10)
        {
            var currentDate = DateTime.UtcNow;

            var pipeline = new BsonDocument[]
            {
        new BsonDocument("$match",
            new BsonDocument
            {
                { "publishDate", new BsonDocument("$gt", currentDate) }
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
                { "endDate", 1 },
                { "genre._id", 1 },
                { "genre.name", 1 },
                { "type", 1 },
                { "actors", 1 },
                { "director", 1 },
                { "description", 1 },
                { "image", 1 },
                { "trailer", 1 },
                { "duration", 1 },
                { "profit", 1 },
            }
        ),
        new BsonDocument("$sort",
            new BsonDocument
            {
                { "publishDate", 1 }
            }
        ),
        new BsonDocument("$skip", (page - 1) * pageSize),
        new BsonDocument("$limit", pageSize),
            };

            var totalUpcomingMovies = await _movieCollection.CountDocumentsAsync(new BsonDocument());

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _movieCollection.Aggregate<MovieWithGenre>(pipeline, options).ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalUpcomingMovies / pageSize);

            var pagedResult = new PagedResult<MovieWithGenre>
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Data = result
            };

            return pagedResult;
        }

        public async Task<PagedResult<MovieWithGenre>> GetCurrentAsync(int page = 1, int pageSize = 10)
        {
            var currentDate = DateTime.UtcNow;

            var pipeline = new BsonDocument[]
            {
        new BsonDocument("$match",
            new BsonDocument
            {
                { "publishDate", new BsonDocument("$lte", currentDate) },
                { "endDate", new BsonDocument("$gte", currentDate) }
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
                { "endDate", 1 },
                { "genre._id", 1 },
                { "genre.name", 1 },
                { "type", 1 },
                { "actors", 1 },
                { "director", 1 },
                { "description", 1 },
                { "image", 1 },
                { "trailer", 1 },
                { "duration", 1 },
                { "profit", 1 },
            }
        ),
        new BsonDocument("$sort",
            new BsonDocument
            {
                { "publishDate", 1 }
            }
        ),
        new BsonDocument("$skip", (page - 1) * pageSize),
        new BsonDocument("$limit", pageSize),
            };

            var totalUpcomingMovies = await _movieCollection.CountDocumentsAsync(new BsonDocument());

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _movieCollection.Aggregate<MovieWithGenre>(pipeline, options).ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalUpcomingMovies / pageSize);

            var pagedResult = new PagedResult<MovieWithGenre>
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Data = result
            };

            return pagedResult;
        }


        public async Task<PagedResult<MovieWithGenre>> GetAll(int page = 1, int pageSize = 10)
        {
            var currentDate = DateTime.UtcNow;

            var pipeline = new BsonDocument[]
            {
        new BsonDocument("$match",
            new BsonDocument
            {
                {
                    "$or", new BsonArray
                    {
                        new BsonDocument
                        {
                            { "publishDate", new BsonDocument("$lte", currentDate) },
                            { "endDate", new BsonDocument("$gte", currentDate) }
                        },
                        new BsonDocument
                        {
                            { "publishDate", new BsonDocument("$gt", currentDate) }
                        }
                    }
                }
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
                { "endDate", 1 },
                { "genre._id", 1 },
                { "genre.name", 1 },
                { "type", 1 },
                { "actors", 1 },
                { "director", 1 },
                { "description", 1 },
                { "image", 1 },
                { "trailer", 1 },
                { "duration", 1 },
                { "profit", 1 },
            }
        ),
        new BsonDocument("$sort",
            new BsonDocument
            {
                { "publishDate", 1 }
            }
        ),
        new BsonDocument("$skip", (page - 1) * pageSize),
        new BsonDocument("$limit", pageSize),
            };

            var totalMovies = await _movieCollection.CountDocumentsAsync(
                new BsonDocument { }
            );

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _movieCollection.Aggregate<MovieWithGenre>(pipeline, options).ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalMovies / pageSize);

            var pagedResult = new PagedResult<MovieWithGenre>
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
            FilterDefinition<Movie> filter = Builders<Movie>.Filter.Eq("Id", id);
            await _movieCollection.DeleteOneAsync(filter);
            return;
        }

        public int[] GetTotalFilmsPerMonth()
        {
            var pipeline = new List<BsonDocument>
            {
                BsonDocument.Parse("{ $group: { _id: { $month: '$publishDate' }, count: { $sum: 1 } } }"),
                BsonDocument.Parse("{ $sort: { '_id': 1 } }")
            };

            var aggregationResult = _movieCollection.Aggregate<BsonDocument>(pipeline).ToList();

            var monthlyTotals = new int[12]; 

            foreach (var result in aggregationResult)
            {
                var month = result["_id"].AsInt32;
                var count = result["count"].AsInt32;

                monthlyTotals[month - 1] = count;
            }

            return monthlyTotals;
        }

        public Dictionary<string, List<object>> GetMoviesProfit()
        {
            var filter = Builders<Movie>.Filter.Gt("profit", 0);
            var films = _movieCollection.Find(filter).ToList();

            var result = new Dictionary<string, List<object>>();

            var names = new List<object>();
            var profits = new List<object>();

            foreach (var film in films)
            {
                names.Add(film.name);
                profits.Add(film.profit);
            }

            result.Add("names", names);
            result.Add("profits", profits);
            return result;
        }

        public List<Movie> GetTop10FilmsByProfit()
        {
            var sortDefinition = Builders<Movie>.Sort.Descending(f => f.profit);
            var films = _movieCollection.Find(Builders<Movie>.Filter.Empty).Sort(sortDefinition).Limit(10).ToList();

            return films;
        }

        public async Task<int> GetTotalMoviesQuantity()
        {
            try
            {
                var totalQuantity = await _movieCollection.CountDocumentsAsync(_ => true);
                return (int)totalQuantity;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error getting total movies quantity: {ex.Message}");
            }
        }
    }
}
