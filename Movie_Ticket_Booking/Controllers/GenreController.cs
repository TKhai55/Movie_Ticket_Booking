using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Service;

namespace Movie_Ticket_Booking.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class GenreController : Controller
    {

        private readonly GenreService _mongoDBService;

        public GenreController(GenreService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Genre>>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var seats = await _mongoDBService.GetAsync(page, pageSize);
            return Ok(seats);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Genre genre)
        {
            await _mongoDBService.CreateAsync(genre);
            return CreatedAtAction(nameof(Get), new { id = genre.Id }, genre);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Genre>> GetById(string id)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format");
            }

            var genre = await _mongoDBService.GetByIdAsync(id);
            if (genre == null)
            {
                return NotFound("Genre not found");
            }

            return Ok(genre);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Genre updatedGenre)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format");
            }

            try
            {
                await _mongoDBService.UpdateAsync(id, updatedGenre);
                return Ok("Genre updated successfully");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message); // Trả về lỗi nếu không tìm thấy hoặc có lỗi trong quá trình cập nhật
            }
        }
        [HttpGet("search")]
        public async Task<ActionResult<List<Genre>>> SearchGenre([FromQuery] string genre)
        {
            if (string.IsNullOrEmpty(genre))
            {
                return BadRequest("Invalid account");
            }

            var result = await _mongoDBService.SearchAsync(genre);
            if (result == null)
            {
                return NotFound("User not found");
            }

            return Ok(result);
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
