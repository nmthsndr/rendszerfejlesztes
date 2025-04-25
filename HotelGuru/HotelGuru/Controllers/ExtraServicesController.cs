using Microsoft.AspNetCore.Mvc;
using HotelGuru.Services;
using HotelGuru.DataContext.Dtos;

namespace HotelGuru.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExtraServicesController : ControllerBase
    {
        private readonly IExtraServiceService _extraServiceService;

        public ExtraServicesController(IExtraServiceService extraServiceService)
        {
            _extraServiceService = extraServiceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExtraServices()
        {
            var extraServices = await _extraServiceService.GetAllExtraServicesAsync();
            return Ok(extraServices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExtraServiceById(int id)
        {
            var extraService = await _extraServiceService.GetExtraServiceByIdAsync(id);
            if (extraService == null)
            {
                return NotFound();
            }
            return Ok(extraService);
        }

        [HttpPost]
        public async Task<IActionResult> CreateExtraService([FromBody] ExtraServiceDto extraServiceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdExtraService = await _extraServiceService.CreateExtraServiceAsync(extraServiceDto);
            return CreatedAtAction(nameof(GetExtraServiceById), new { id = createdExtraService.Id }, createdExtraService);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExtraService(int id, [FromBody] ExtraServiceDto extraServiceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedExtraService = await _extraServiceService.UpdateExtraServiceAsync(id, extraServiceDto);
            if (updatedExtraService == null)
            {
                return NotFound();
            }
            return Ok(updatedExtraService);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExtraService(int id)
        {
            var result = await _extraServiceService.DeleteExtraServiceAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}