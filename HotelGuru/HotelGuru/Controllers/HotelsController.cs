using Microsoft.AspNetCore.Mvc;
using HotelGuru.Services;
using HotelGuru.DataContext.Dtos;

namespace HotelGuru.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHotels()
        {
            var hotels = await _hotelService.GetAllHotelsAsync();
            return Ok(hotels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHotelById(int id)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }
            return Ok(hotel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateHotel([FromBody] HotelDto hotelDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdHotel = await _hotelService.CreateHotelAsync(hotelDto);
            return CreatedAtAction(nameof(GetHotelById), new { id = createdHotel.Id }, createdHotel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] HotelDto hotelDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedHotel = await _hotelService.UpdateHotelAsync(id, hotelDto);
            if (updatedHotel == null)
            {
                return NotFound();
            }
            return Ok(updatedHotel);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var result = await _hotelService.DeleteHotelAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}