using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Service;

namespace Movie_Ticket_Booking.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class NewsController : Controller
    {

        private readonly NewsService _mongoDBService;

        public NewsController(NewsService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet]
        public async Task<ActionResult<List<NewsWithCreator>>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var seats = await _mongoDBService.GetAsync(page, pageSize);
            return Ok(seats);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NewsWithCreator>> GetById(string id)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format");
            }

            var news = await _mongoDBService.GetByIdAsync(id);
            if (news == null)
            {
                return NotFound("News not found");
            }

            return Ok(news);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] News news)
        {
            await _mongoDBService.CreateAsync(news);
            return CreatedAtAction(nameof(Get), new { id = news.Id }, news);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] News updatedNews)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format");
            }

            try
            {
                await _mongoDBService.UpdateAsync(id, updatedNews);
                return Ok("News updated successfully");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message); // Trả về lỗi nếu không tìm thấy hoặc có lỗi trong quá trình cập nhật
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<Voucher>>> SearchAsync([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                {
                    return BadRequest("Invalid title");
                }

                var pagedResult = await _mongoDBService.SearchAsync(query, page, pageSize);

                if (pagedResult.Data == null || pagedResult.Data.Count == 0)
                {
                    return NotFound("News not found");
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteAsync(id);
            return Ok("Delete successfully");

        }

    }
}
