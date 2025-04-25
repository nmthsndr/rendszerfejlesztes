using Microsoft.AspNetCore.Mvc;
using HotelGuru.Services;
using HotelGuru.DataContext.Dtos;

namespace HotelGuru.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAddresses()
        {
            var addresses = await _addressService.GetAllAddressesAsync();
            return Ok(addresses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressById(int id)
        {
            var address = await _addressService.GetAddressByIdAsync(id);
            if (address == null)
            {
                return NotFound();
            }
            return Ok(address);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAddressesByUserId(int userId)
        {
            var addresses = await _addressService.GetAddressesByUserIdAsync(userId);
            return Ok(addresses);
        }

        [HttpPost("user/{userId}")]
        public async Task<IActionResult> CreateAddress(int userId, [FromBody] AddressDto addressDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdAddress = await _addressService.CreateAddressAsync(addressDto, userId);
            return CreatedAtAction(nameof(GetAddressById), new { id = createdAddress.Id }, createdAddress);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressDto addressDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedAddress = await _addressService.UpdateAddressAsync(id, addressDto);
            if (updatedAddress == null)
            {
                return NotFound();
            }
            return Ok(updatedAddress);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var result = await _addressService.DeleteAddressAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}