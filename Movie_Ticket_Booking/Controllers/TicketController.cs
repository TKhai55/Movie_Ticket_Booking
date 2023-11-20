using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Service;

namespace Movie_Ticket_Booking.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class TicketController : Controller
    {

        private readonly TicketService _mongoDBService;

        public TicketController(TicketService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet]
        public async Task<List<TicketInformation>> Get()
        {
            return await _mongoDBService.GetAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Ticket ticket)
        {
            await _mongoDBService.CreateAsync(ticket);
            return CreatedAtAction(nameof(Get), new { id = ticket.Id }, ticket);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketInformation>> GetById(string id)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format");
            }

            var ticket = await _mongoDBService.GetByIdAsync(id);
            if (ticket == null)
            {
                return NotFound("Ticket not found");
            }

            return Ok(ticket);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteAsync(id);
            return NoContent();
        }

    }
}
