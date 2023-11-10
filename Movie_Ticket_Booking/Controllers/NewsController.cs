using Microsoft.AspNetCore.Mvc;
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
        public async Task<List<News>> Get()
        {
            return await _mongoDBService.GetAsync();
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
