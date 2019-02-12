using Microsoft.AspNetCore.Mvc;

namespace ChargingGatewayApi.Controllers
{
    // [Authorize(Roles = "sample_api.admin")]
    [Route("v1/[controller]")]
    public class ValuesController : Controller
    {
        // GET
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new[] {"value1", "value2"});
        }

        // GET 
        [HttpGet("{id}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            return Ok("value");
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            return CreatedAtRoute("GetById", new {id = 1}, value);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string value)
        {
            return new NoContentResult();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return new NoContentResult();
        }
    }
}