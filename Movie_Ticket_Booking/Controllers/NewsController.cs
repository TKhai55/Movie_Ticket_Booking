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
        public async Task<List<NewsWithCreator>> Get()
        {
            return await _mongoDBService.GetAsync();
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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] News news)
        {
            await _mongoDBService.CreateAsync(news);
            return CreatedAtAction(nameof(Get), new { id = news.Id }, news);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteAsync(id);
            return NoContent();
        }

    }
}
