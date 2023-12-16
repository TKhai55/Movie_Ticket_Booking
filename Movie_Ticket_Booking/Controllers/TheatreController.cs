using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<List<Theatre>>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var seats = await _mongoDBService.GetAsync(page, pageSize);
            return Ok(seats);
        }

        [Authorize]
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

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<Genre>>> SearchTheatre([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                {
                    return BadRequest("Invalid name");
                }

                var pagedResult = await _mongoDBService.SearchAsync(query, page, pageSize);

                if (pagedResult.Data == null || pagedResult.Data.Count == 0)
                {
                    return NotFound("Theatre not found");
                }

                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
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

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteAsync(id);
            return Ok("Delete successfully");

        }

        [HttpGet("total-quantity")]
        public async Task<IActionResult> GetTotalTheatresQuantityAsync()
        {
            try
            {
                var totalQuantity = await _mongoDBService.GetTotalTheatreQuantity();

                return Ok(new { TotalQuantity = totalQuantity });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
