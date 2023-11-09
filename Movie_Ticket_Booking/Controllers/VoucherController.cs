using Microsoft.AspNetCore.Mvc;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Service;

namespace Movie_Ticket_Booking.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class VoucherController : Controller
    {

        private readonly VoucherService _mongoDBService;

        public VoucherController(VoucherService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet]
        public async Task<List<Voucher>> Get()
        {
            return await _mongoDBService.GetAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Voucher voucher)
        {
            await _mongoDBService.CreateAsync(voucher);
            return CreatedAtAction(nameof(Get), new { id = voucher.Id }, voucher);
        }

        /*[HttpPut("{id}")]
        public async Task<IActionResult> AddTovoucher(string id, [FromBody] string movieId)
        {
            await _mongoDBService.AddTovoucherAsync(id, movieId);
            return NoContent();
        }*/

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteAsync(id);
            return NoContent();
        }

    }
}
