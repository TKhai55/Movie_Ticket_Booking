using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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
        [HttpGet("{id}")]
        public async Task<ActionResult<Voucher>> GetById(string id)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format");
            }

            var voucher = await _mongoDBService.GetByIdAsync(id);
            if (voucher == null)
            {
                return NotFound("News not found");
            }

            return Ok(voucher);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Voucher updatedVoucher)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format");
            }

            try
            {
                await _mongoDBService.UpdateAsync(id, updatedVoucher);
                return Ok("Voucher updated successfully");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message); // Trả về lỗi nếu không tìm thấy hoặc có lỗi trong quá trình cập nhật
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteAsync(id);
            return NoContent();
        }

    }
}
