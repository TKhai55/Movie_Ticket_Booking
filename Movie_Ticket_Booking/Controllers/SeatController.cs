using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Service;

namespace Movie_Ticket_Booking.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class SeatController : Controller
    {

        private readonly SeatService _mongoDBService;

        public SeatController(SeatService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet]
        public async Task<List<SeatInfo>> Get()
        {
            return await _mongoDBService.GetAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Seat seat)
        {
            await _mongoDBService.CreateAsync(seat);
            return CreatedAtAction(nameof(Get), new { id = seat.Id }, seat);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SeatInfo>> GetById(string id)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format");
            }

            var seat = await _mongoDBService.GetByIdAsync(id);
            if (seat == null)
            {
                return NotFound("News not found");
            }

            return Ok(seat);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteAsync(id);
            return NoContent();
        }

    }
}
