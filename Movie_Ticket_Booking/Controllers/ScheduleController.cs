using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<List<ScheduleFullinfo>>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var seats = await _mongoDBService.GetAsync(page, pageSize);
            return Ok(seats);
        }
        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<ScheduleFullinfo>>> SearchSchedules([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mongoDBService.SearchAsync(query, page, pageSize);
            return Ok(result);
        }
        [Authorize]
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

        [HttpGet("searchByMovieId")]
        public async Task<ActionResult<List<ScheduleFullinfo>>> GetByMovieId([FromQuery] string movieId)
        {
            if (string.IsNullOrEmpty(movieId) || !ObjectId.TryParse(movieId, out _))
            {
                return BadRequest("Invalid ID format");
            }

            var schedule = await _mongoDBService.GetByMovieIdAsync(movieId);
            if (schedule == null)
            {
                return NotFound("Schedule not found");
            }

            return Ok(schedule);
        }

        [Authorize]
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

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteAsync(id);
            return Ok("Delete successfully");

        }

    }
}
