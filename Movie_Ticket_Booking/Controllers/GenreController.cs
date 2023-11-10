using Microsoft.AspNetCore.Mvc;
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


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteAsync(id);
            return NoContent();
        }

    }
}
