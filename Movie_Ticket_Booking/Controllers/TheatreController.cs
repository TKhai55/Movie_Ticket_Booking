using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Service;

namespace Movie_Ticket_Booking.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class TheatreController : Controller
    {

        private readonly TheatreService _mongoDBService;

        public TheatreController(TheatreService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet]
        public async Task<List<Theatre>> Get()
        {
            return await _mongoDBService.GetAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Theatre theatre)
        {
            await _mongoDBService.CreateAsync(theatre);
            return CreatedAtAction(nameof(Get), new { id = theatre.Id }, theatre);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Theatre>> GetById(string id)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format");
            }

            var theatre = await _mongoDBService.GetByIdAsync(id);
            if (theatre == null)
            {
                return NotFound("News not found");
            }

            return Ok(theatre);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Theatre updatedTheatre)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format");
            }

            try
            {
                await _mongoDBService.UpdateAsync(id, updatedTheatre);
                return Ok("Theatre updated successfully");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message); // Trả về lỗi nếu không tìm thấy hoặc có lỗi trong quá trình cập nhật
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
