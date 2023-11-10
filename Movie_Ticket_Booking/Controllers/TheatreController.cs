using Microsoft.AspNetCore.Mvc;
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


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteAsync(id);
            return NoContent();
        }

    }
}
