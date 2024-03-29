﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Service;

namespace Movie_Ticket_Booking.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class SeatController : Controller
    {

        private readonly SeatService _mongoDBService;

        public SeatController(SeatService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet]
        public async Task<ActionResult<List<SeatInfo>>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var seats = await _mongoDBService.GetAsync(page, pageSize);
            return Ok(seats);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Seat seat)
        {
            await _mongoDBService.CreateAsync(seat);
            return CreatedAtAction(nameof(Get), new { id = seat.Id }, seat);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SeatInfo>> GetById(string id)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid ID format");
            }

            var seat = await _mongoDBService.GetByIdAsync(id);
            if (seat == null)
            {
                return NotFound("News not found");
            }

            return Ok(seat);
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
