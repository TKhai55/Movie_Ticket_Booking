using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Movie_Ticket_Booking.Models;

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

        public async Task<List<Voucher>> GetAsync()
        {
            return await _voucherCollection.Find(new BsonDocument()).ToListAsync();
        }
        public async Task CreateAsync(Voucher voucher)
        {
            await _voucherCollection.InsertOneAsync(voucher);
            return;
        }

        public async Task<Voucher> GetByIdAsync(string id)
        {
            var voucher = await _voucherCollection.Find(t => t.Id == id).FirstOrDefaultAsync();
            return voucher;
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
