using Microsoft.AspNetCore.Mvc;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Service;

namespace Movie_Ticket_Booking.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class UserController : Controller
    {

        private readonly UserService _mongoDBService;

        public UserController(UserService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet]
        public async Task<List<User>> Get()
        {
            return await _mongoDBService.GetAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User account)
        {
            await _mongoDBService.CreateAsync(account);
            return CreatedAtAction(nameof(Get), new { id = account.Id }, account);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteAsync(id);
            return NoContent();
        }

    }
}
