using AutoMapper;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.Models.Hotel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelsController : ControllerBase
{
    private readonly IHotelsRepository _hotelsRepository;
    private readonly IMapper _mapper;

    public HotelsController(IHotelsRepository hotelsRepository, IMapper mapper)
    {
        this._hotelsRepository = hotelsRepository;
        this._mapper = mapper;
    }

    // GET: api/<HotelsController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotelDto>>> Get()
    {
        var hotels = await _hotelsRepository.GetAllAsync();
        var hotelsDto = _mapper.Map<List<HotelDto>>(hotels);

        return Ok(hotelsDto);
    }

    // GET api/<HotelsController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult<HotelDto>> Get(int id)
    {
        var hotel = await _hotelsRepository.GetAsync(id);

        if (hotel == null)
        {
            return NotFound();
        }

        var hotelDto = _mapper.Map<HotelDto>(hotel);

        return Ok(hotelDto);
    }

    // POST api/<HotelsController>
    [HttpPost]
    public async Task<ActionResult<Hotel>> Post([FromBody] CreateHotelDto createHotelDto)
    {
        var newHotel = _mapper.Map<Hotel>(createHotelDto);
        await _hotelsRepository.AddAsync(newHotel);

        return CreatedAtAction(nameof(Get), new { id = newHotel.Id }, newHotel);
    }

    // PUT api/<HotelsController>/5
    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] HotelDto updatedHoel)
    {
        if (id != updatedHoel.Id)
        {
            return BadRequest("Invalid Record Id");
        }

        var hotel = await _hotelsRepository.GetAsync(id);

        if (hotel == null)
        {
            return NotFound();
        }

        _mapper.Map(updatedHoel, hotel);

        try
        {
            await _hotelsRepository.UpdateAsync(hotel);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await HotelExists(id))
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
        var hotel = await _hotelsRepository.GetAsync(id);
        if (hotel == null)
        {
            return NotFound();
        }

        await _hotelsRepository.DeleteAsync(id);

        return NoContent();
    }

    private async Task<bool> HotelExists(int id)
    {
        return await _hotelsRepository.Exists(id);
    }
}
