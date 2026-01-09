using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace CustomerValidationSystem.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ExampleController : ControllerBase
{
    // GET: api/v1/example
    [HttpGet]
    public ActionResult<IEnumerable<string>> Get()
    {
        // Returns a list of example values
        var examples = new[] { "example1", "example2" };
        return this.Ok(examples);
    }

    // GET: api/v1/example/5
    [HttpGet("{id}")]
    public ActionResult<string> Get(int id)
    {
        // Returns a single example value based on ID
        return this.Ok($"example{id}");
    }

    // POST: api/v1/example
    [HttpPost]
    public IActionResult Post([FromBody] string value)
    {
        // Simulates creating a new resource
        return this.CreatedAtAction(nameof(Get), new { id = 1 }, value);
    }

    // PUT: api/v1/example/5
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] string value)
    {
        // Simulates updating a resource
        return this.NoContent();
    }

    // DELETE: api/v1/example/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        // Simulates deleting a resource
        return this.NoContent();
    }
}
