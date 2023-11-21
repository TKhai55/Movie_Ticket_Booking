﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("searchBasic")]
        public async Task<ActionResult<List<MovieWithGenre>>> Search(
            [FromQuery(Name = "query")] string query)
        {
            var result = await _mongoDBService.SearchAsync(query);
            return Ok(result);
        }
        [HttpGet("searchByGenre")]
        public async Task<ActionResult<List<MovieWithGenre>>> SearchByGenre(
    [FromQuery(Name = "genreId")] string genreId)
        {
            if (!ObjectId.TryParse(genreId, out ObjectId objectId))
            {
                // Handle invalid ObjectId
                return BadRequest("Invalid genreId format");
            }

            var result = await _mongoDBService.SearchByGenreAsync(objectId);
            return Ok(result);
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
