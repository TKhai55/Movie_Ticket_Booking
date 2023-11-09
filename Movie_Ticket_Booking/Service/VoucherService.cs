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
        /*        public async Task AddTovoucherAsync(string id, string movieId)
                {
                    FilterDefinition<Voucher> filter = Builders<Voucher>.Filter.Eq("Id", id);
                    UpdateDefinition<Voucher> update = Builders<Voucher>.Update.AddToSet<string>("movieIds", movieId);
                    await _voucherCollection.UpdateOneAsync(filter, update);
                    return;
                }*/
        public async Task DeleteAsync(string id)
        {
            FilterDefinition<Voucher> filter = Builders<Voucher>.Filter.Eq("Id", id);
            await _voucherCollection.DeleteOneAsync(filter);
            return;
        }
    }
}
