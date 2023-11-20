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
            var existingAccount = await _accountCollection.Find(a => a.account == account.account).FirstOrDefaultAsync();

            if (existingAccount != null)
            {
                throw new Exception("An account with the same username or email already exists.");
            }

            // If no existing account found, insert the new account
            await _accountCollection.InsertOneAsync(account);
            return;
        }

        public async Task<User> GetByIdAsync(string id)
        {
            var user = await _accountCollection.Find(t => t.Id == id).FirstOrDefaultAsync();
            return user;
        }

        public async Task UpdateAsync(string id, User updatedUser)
        {
            var filter = Builders<User>.Filter.Eq(user => user.Id, id);
            var updateBuilder = Builders<User>.Update;

            var updateDefinition = updateBuilder.Set(user => user.Id, id);

            if (updatedUser.password != null)
                updateDefinition = updateDefinition.Set(movie => movie.password, updatedUser.password);


            await _accountCollection.UpdateOneAsync(filter, updateDefinition);
        }
        public async Task DeleteAsync(string id)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq("Id", id);
            await _accountCollection.DeleteOneAsync(filter);
            return;
        }
    }
}
