using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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
        public async Task<List<ScheduleFullinfo>> Get()
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
        [HttpGet("{id}")]
        public async Task<ActionResult<ScheduleFullinfo>> GetById(string id)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format");
            }

            var schedule = await _mongoDBService.GetByIdAsync(id);
            if (schedule == null)
            {
                return NotFound("Schedule not found");
            }

            return Ok(schedule);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Schedule updatedSchedule)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format");
            }

            try
            {
                var (success, message) = await _mongoDBService.UpdateAsync(id, updatedSchedule);

                if (success)
                {
                    return Ok(message);
                }
                else
                {
                    return BadRequest(message);
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteAsync(id);
            return NoContent();
        }

    }
}
