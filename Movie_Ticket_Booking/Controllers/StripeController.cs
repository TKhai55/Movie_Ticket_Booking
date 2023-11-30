using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Movie_Ticket_Booking.Controllers
{
    [Route("api/[controller]")]
    public class StripeController : Controller
    {
        // GET: api/stripe
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/stripe/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // GET api/stripe
        [HttpGet]
        public async Task<IActionResult> Post()
        {
            // $10
            var result = await Task.FromResult("https://buy.stripe.com/test_eVabMq0X06hp95ScMM");

            return Ok(result);
        }

        // PUT api/stripe/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/stripe/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}

