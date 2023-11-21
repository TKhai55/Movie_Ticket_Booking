using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User loginModel)
        {
            var user = await _mongoDBService.AuthenticateAsync(loginModel.account, loginModel.password);

            if (user == null)
                return BadRequest(new { Message = "Invalid username or password" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr"); // Same key as in Startup.cs
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.Name, user.Id),
                    // Add additional claims as needed
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<PagedResult<User>>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedVouchers = await _mongoDBService.GetAsync(page, pageSize);
            return Ok(pagedVouchers);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User account)
        {
            try
            {
                await _mongoDBService.CreateAsync(account);
                return CreatedAtAction(nameof(Get), new { id = account.Id }, account);
            }
            catch (Exception ex)
            {
                // Handle the exception, for example, log it or return an error response
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(string id)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format");
            }

            var user = await _mongoDBService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] User updatedUser)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format");
            }

            try
            {
                await _mongoDBService.UpdateAsync(id, updatedUser);
                return Ok("User updated successfully");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message); // Trả về lỗi nếu không tìm thấy hoặc có lỗi trong quá trình cập nhật
            }
        }
        [HttpGet("search")]
        public async Task<ActionResult<List<Voucher>>> GetByVoucherDefault([FromQuery] string account)
        {
            if (string.IsNullOrEmpty(account))
            {
                return BadRequest("Invalid account");
            }

            var accounts = await _mongoDBService.SearchAsync(account);
            if (accounts == null || accounts.Count == 0)
            {
                return NotFound("User not found");
            }

            return Ok(accounts);
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
