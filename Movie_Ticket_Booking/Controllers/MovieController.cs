using Microsoft.AspNetCore.Mvc;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Service;

namespace Movie_Ticket_Booking.Controllers
{

    [Controller]
    [Route("api/[controller]")]
    public class MovieController : Controller
    {

        private readonly MovieService _mongoDBService;

        public MovieController(MovieService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet]
        public async Task<List<Movie>> Get()
        {
            return await _mongoDBService.GetAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Movie movie)
        {
            await _mongoDBService.CreateAsync(movie);
            return CreatedAtAction(nameof(Get), new { id = movie.Id }, movie);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteAsync(id);
            return NoContent();
        }

    }
}
