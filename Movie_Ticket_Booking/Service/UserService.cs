using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Movie_Ticket_Booking.Models;

namespace Movie_Ticket_Booking.Service
{
    public class UserService
    {
        private readonly IMongoCollection<User> _accountCollection;

        public UserService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _accountCollection = database.GetCollection<User>("account");
        }

        public async Task<List<User>> GetAsync()
        {
            return await _accountCollection.Find(new BsonDocument()).ToListAsync();
        }
        public async Task CreateAsync(User account)
        {
            await _accountCollection.InsertOneAsync(account);
            return;
        }

        public async Task DeleteAsync(string id)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq("Id", id);
            await _accountCollection.DeleteOneAsync(filter);
            return;
        }
    }
}
