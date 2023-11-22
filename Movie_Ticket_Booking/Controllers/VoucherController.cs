using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<PagedResult<Voucher>>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedVouchers = await _mongoDBService.GetAsync(page, pageSize);
            return Ok(pagedVouchers);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Voucher voucher)
        {
            try
            {

            await _mongoDBService.CreateAsync(voucher);
            return CreatedAtAction(nameof(Get), new { id = voucher.Id }, voucher);
            }
            catch (Exception ex)
            {
                // Handle the exception, for example, log it or return an error response
                return BadRequest(new { Message = ex.Message });
            }
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
                return NotFound("Voucher not found");
            }

            return Ok(voucher);
        }

        [HttpGet("searchByCodeCustomer")]
        public async Task<ActionResult<Voucher>> GetByVoucher([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Invalid code");
            }

            var voucher = await _mongoDBService.GetByVoucherCustomerAsync(code);
            if (voucher == null)
            {
                return NotFound("Voucher not found");
            }

            return Ok(voucher);
        }

        [HttpGet("searchBasic")]
        public async Task<ActionResult<PagedResult<Voucher>>> GetByVoucherDefault([FromQuery] string code, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return BadRequest("Invalid code");
                }

                var pagedResult = await _mongoDBService.GetByVoucherBasicAsync(code, page, pageSize);

                if (pagedResult.Data == null || pagedResult.Data.Count == 0)
                {
                    return NotFound("Voucher not found");
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

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteAsync(id);
            return Ok("Delete successfully");
        }

    }
}
