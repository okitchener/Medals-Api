using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

[ApiController, Route("[controller]/country")]
public class ApiController(DataContext db) : ControllerBase
{
    private readonly DataContext _dataContext = db;

    // http get entire collection
    [HttpGet, SwaggerOperation(summary: "return entire collection", null)]
    public async Task<ActionResult<IEnumerable<Country>>> Get()
    {
        return Ok(await _dataContext.Countries.ToListAsync());
    }

    // http get specific member of collection
    [HttpGet("{id}"), SwaggerOperation(summary: "return specific member of collection", null)]
    [ProducesResponseType(typeof(Country), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Country>> Get(int id)
    {
        Country? country = await _dataContext.Countries.FindAsync(id);
        if (country == null)
        {
            return NotFound();
        }
        return Ok(country);
    }

    // http post member to collection
    [HttpPost, SwaggerOperation(summary: "add member to collection", null)]
    [ProducesResponseType(typeof(Country), StatusCodes.Status201Created)]
    [SwaggerResponse(StatusCodes.Status201Created, "Created")]
    public async Task<ActionResult<Country>> Post([FromBody] Country country)
    {
        _dataContext.Countries.Add(country);
        await _dataContext.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = country.Id }, country);
    }

    // http delete member from collection
    [HttpDelete("{id}"), SwaggerOperation(summary: "delete member from collection", null)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status204NoContent, "No Content")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found")]
    public async Task<ActionResult> Delete(int id)
    {
        Country? country = await _dataContext.Countries.FindAsync(id);
        if (country == null)
        {
            return NotFound();
        }
        _dataContext.Countries.Remove(country);
        await _dataContext.SaveChangesAsync();
        return NoContent();
    }
}
