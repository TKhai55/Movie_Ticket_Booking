using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Movie_Ticket_Booking.Models;
using System.Text.RegularExpressions;

namespace Movie_Ticket_Booking.Service
{
    public class VoucherService
    {
        private readonly IMongoCollection<Voucher> _voucherCollection;

        public VoucherService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _voucherCollection = database.GetCollection<Voucher>("voucher");
        }

        public async Task<PagedResult<Voucher>> GetAsync(int page = 1, int pageSize = 10)
        {
            var totalVouchers = await _voucherCollection.CountDocumentsAsync(new BsonDocument());

            var pipeline = new BsonDocument[]
            {
                 new BsonDocument("$skip", (page - 1) * pageSize),
                     new BsonDocument("$limit", pageSize),
            };

            var totalSeats = await _voucherCollection.CountDocumentsAsync(new BsonDocument());

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _voucherCollection.Aggregate<Voucher>(pipeline, options).ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalSeats / pageSize);

            var pagedResult = new PagedResult<Voucher>
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Data = result
            };

            return pagedResult;
        }
        public async Task<bool> IsCodeUniqueAsync(string code)
        {
            var existingVoucher = await _voucherCollection.Find(x => x.code == code).FirstOrDefaultAsync();
            return existingVoucher == null;
        }

        public async Task CreateAsync(Voucher voucher)
        {
            if (await IsCodeUniqueAsync(voucher.code))
            {
                await _voucherCollection.InsertOneAsync(voucher);
            }
            else
            {
                throw new Exception("An voucher with the same code already exists.");
            }
            return;
        }

        public async Task<Voucher> GetByIdAsync(string id)
        {
            var voucher = await _voucherCollection.Find(t => t.Id == id).FirstOrDefaultAsync();
            return voucher;
        }

        public async Task<Voucher> GetByVoucherCustomerAsync(string code)
        {
            var filter = Builders<Voucher>.Filter.Eq(t => t.code, code);
            var voucher = await _voucherCollection.Find(filter).FirstOrDefaultAsync();
            return voucher;
        }

        public async Task<PagedResult<Voucher>> GetByVoucherBasicAsync(string code, int page = 1, int pageSize = 10)
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("code", new BsonRegularExpression(new Regex(code, RegexOptions.IgnoreCase)))),
                new BsonDocument("$skip", (page - 1) * pageSize),
                new BsonDocument("$limit", pageSize),
            };

            var options = new AggregateOptions { AllowDiskUse = false };
            var result = await _voucherCollection.Aggregate<Voucher>(pipeline, options).ToListAsync();

            var totalItems = await _voucherCollection.CountDocumentsAsync(new BsonDocument("code", new BsonRegularExpression(new Regex(code, RegexOptions.IgnoreCase))));
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var pagedResult = new PagedResult<Voucher>
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Data = result
            };

            return pagedResult;
        }

        public async Task UpdateAsync(string id, Voucher updatedVoucher)
        {
            var filter = Builders<Voucher>.Filter.Eq(voucher => voucher.Id, id);
            var updateBuilder = Builders<Voucher>.Update;

            var updateDefinition = updateBuilder.Set(voucher => voucher.Id, id);

            if (updatedVoucher.name != null)
                updateDefinition = updateDefinition.Set(voucher => voucher.name, updatedVoucher.name);

            if (updatedVoucher.description != null)
                updateDefinition = updateDefinition.Set(voucher => voucher.description, updatedVoucher.description);
            if (updatedVoucher.value != default)
                updateDefinition = updateDefinition.Set(voucher => voucher.value, updatedVoucher.value);

            await _voucherCollection.UpdateOneAsync(filter, updateDefinition);
        }
        public async Task DeleteAsync(string id)
        {
            FilterDefinition<Voucher> filter = Builders<Voucher>.Filter.Eq("Id", id);
            await _voucherCollection.DeleteOneAsync(filter);
            return;
        }
    }
}
