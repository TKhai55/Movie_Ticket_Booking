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
        public async Task<List<Genre>> Get()
        {
            return await _mongoDBService.GetAsync();
        }

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteAsync(id);
            return NoContent();
        }

    }
}
