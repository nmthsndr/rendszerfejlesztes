using Microsoft.AspNetCore.Mvc;
using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelGuru.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExtraServicesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExtraServicesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExtraServices()
        {
            var extraServices = await _context.ExtraServices.ToListAsync();
            return Ok(extraServices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExtraServiceById(int id)
        {
            var extraService = await _context.ExtraServices.FindAsync(id);
            if (extraService == null)
            {
                return NotFound();
            }
            return Ok(extraService);
        }

        [HttpPost]
        public async Task<IActionResult> CreateExtraService([FromBody] ExtraService extraService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ExtraServices.Add(extraService);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetExtraServiceById), new { id = extraService.Id }, extraService);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExtraService(int id, [FromBody] ExtraService extraService)
        {
            if (id != extraService.Id)
            {
                return BadRequest();
            }

            _context.Entry(extraService).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ExtraServices.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExtraService(int id)
        {
            var extraService = await _context.ExtraServices.FindAsync(id);
            if (extraService == null)
            {
                return NotFound();
            }

            _context.ExtraServices.Remove(extraService);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}