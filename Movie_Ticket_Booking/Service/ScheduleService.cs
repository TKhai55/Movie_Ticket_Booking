using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Movie_Ticket_Booking.Models;

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

        public async Task<List<Schedule>> GetAsync()
        {
            return await _scheduleCollection.Find(new BsonDocument()).ToListAsync();
        }
        public async Task CreateAsync(Schedule schedule)
        {
            // Đảm bảo định dạng ngày và giờ là UTC trước khi lưu trữ
            schedule.startTime = DateTime.SpecifyKind(schedule.startTime, DateTimeKind.Utc);
            schedule.endTime = DateTime.SpecifyKind(schedule.endTime, DateTimeKind.Utc);
            await _scheduleCollection.InsertOneAsync(schedule);
            return;
        }

        public async Task DeleteAsync(string id)
        {
            FilterDefinition<Schedule> filter = Builders<Schedule>.Filter.Eq("Id", id);
            await _scheduleCollection.DeleteOneAsync(filter);
            return;
        }
    }
}

