using HotelListing.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelsController : ControllerBase
{
    private readonly HotelListingDbContext context;

    public HotelsController(HotelListingDbContext context)
    {
        this.context = context;
    }

    // GET: api/<HotelsController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Hotel>>> Get()
    {
        var hotels = await context.Hotels.ToListAsync();
        return Ok(hotels);
    }

    // GET api/<HotelsController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Hotel>> Get(int id)
    {
        var hotel = await context.Hotels.FindAsync(id);

        if (hotel == null)
        {
            return NotFound();
        }

        return Ok(hotel);
    }

    // POST api/<HotelsController>
    [HttpPost]
    public async Task<ActionResult<Hotel>> Post([FromBody] Hotel newHotel)
    {
        context.Hotels.Add(newHotel);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = newHotel.Id }, newHotel);
    }

    // PUT api/<HotelsController>/5
    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] Hotel updatedHoel)
    {
        if (id != updatedHoel.Id)
        {
            return BadRequest("Invalid Record Id");
        }

        context.Entry<Hotel>(updatedHoel).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!HotelExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE api/<HotelsController>/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var hotel = await context.Hotels.FindAsync(id);

        if (hotel == null)
        {
            return NotFound(new { message = "Hotel not found" });
        }

        context.Hotels.Remove(hotel);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool HotelExists(int id)
    {
        return context.Hotels.Any(h => h.Id == id);
    }
}
