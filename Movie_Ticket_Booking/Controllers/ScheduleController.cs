using Microsoft.AspNetCore.Mvc;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Service;

namespace Movie_Ticket_Booking.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class ScheduleController : Controller
    {

        private readonly ScheduleService _mongoDBService;

        public ScheduleController(ScheduleService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet]
        public async Task<List<Schedule>> Get()
        {
            return await _mongoDBService.GetAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Schedule schedule)
        {
            // Đảm bảo định dạng ngày và giờ là UTC trước khi lưu trữ
            schedule.startTime = DateTime.SpecifyKind(schedule.startTime, DateTimeKind.Utc);
            schedule.endTime = DateTime.SpecifyKind(schedule.endTime, DateTimeKind.Utc);
            await _mongoDBService.CreateAsync(schedule);
            return CreatedAtAction(nameof(Get), new { id = schedule.Id }, schedule);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteAsync(id);
            return NoContent();
        }

    }
}
