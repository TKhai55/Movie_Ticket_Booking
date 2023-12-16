using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
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
        public async Task<ActionResult<List<MovieWithGenre>>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var seats = await _mongoDBService.GetAsync(page, pageSize);
            return Ok(seats);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MovieWithGenre>> GetById(string id)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format");
            }

            var news = await _mongoDBService.GetByIdAsync(id);
            if (news == null)
            {
                return NotFound("Movie not found");
            }

            return Ok(news);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Movie movie)
        {
            await _mongoDBService.CreateAsync(movie);
            return CreatedAtAction(nameof(Get), new { id = movie.Id }, movie);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Movie updatedMovie)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format");
            }

            try
            {
                await _mongoDBService.UpdateAsync(id, updatedMovie);
                return Ok("Movie updated successfully");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message); // Trả về lỗi nếu không tìm thấy hoặc có lỗi trong quá trình cập nhật
            }
        }
        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<MovieWithGenre>>> Search([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                {
                    return BadRequest("Invalid format");
                }

                var pagedResult = await _mongoDBService.SearchAsync(query, page, pageSize);

                if (pagedResult.Data == null || pagedResult.Data.Count == 0)
                {
                    return NotFound("Movie not found");
                }

                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("searchGenre")]
        public async Task<ActionResult<PagedResult<MovieWithGenre>>> SearchByGenre(
    [FromQuery(Name = "genreId")] string genreId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (!ObjectId.TryParse(genreId, out ObjectId objectId))
            {
                // Handle invalid ObjectId
                return BadRequest("Invalid genreId format");
            }

            var pagedResult = await _mongoDBService.SearchByGenreAsync(objectId, page, pageSize);
            if (pagedResult.Data == null || pagedResult.Data.Count == 0)
            {
                return NotFound("Movie not found");
            }

            return Ok(pagedResult);
        }

        [HttpGet("getUpcoming")]
        public async Task<ActionResult<PagedResult<MovieWithGenre>>> GetUpcoming(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var pagedResult = await _mongoDBService.GetUpcomingAsync(page, pageSize);

                if (pagedResult.Data == null || pagedResult.Data.Count == 0)
                {
                    return NotFound("No upcoming movies found");
                }

                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("getCurrent")]
        public async Task<ActionResult<PagedResult<MovieWithGenre>>> GetCurrent(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var pagedResult = await _mongoDBService.GetCurrentAsync(page, pageSize);

                if (pagedResult.Data == null || pagedResult.Data.Count == 0)
                {
                    return NotFound("No current movies found");
                }

                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("getCurrent&Upcoming")]
        public async Task<ActionResult<PagedResult<MovieWithGenre>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var pagedResult = await _mongoDBService.GetAll(page, pageSize);

                if (pagedResult.Data == null || pagedResult.Data.Count == 0)
                {
                    return NotFound("No current movies found");
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteAsync(id);
            return Ok("Delete successfully");
        }

        [HttpGet("total-films-per-month")]
        public ActionResult<Dictionary<int, int>> GetTotalFilmsPerMonth()
        {
            var result = _mongoDBService.GetTotalFilmsPerMonth();
            return Ok(result);
        }

        [HttpGet("movies-profit")]
        public ActionResult<List<Movie>> GetFilmsWithProfitGreaterThanZero()
        {
            var films = _mongoDBService.GetMoviesProfit();
            return Ok(films);
        }
    }
}
