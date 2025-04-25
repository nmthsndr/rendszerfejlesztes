using Microsoft.AspNetCore.Mvc;
using HotelGuru.Services;
using HotelGuru.DataContext.Dtos;

namespace HotelGuru.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInvoices()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return Ok(invoices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            return Ok(invoice);
        }

        [HttpGet("reservation/{reservationId}")]
        public async Task<IActionResult> GetInvoiceByReservationId(int reservationId)
        {
            var invoice = await _invoiceService.GetInvoiceByReservationIdAsync(reservationId);
            if (invoice == null)
            {
                return NotFound();
            }
            return Ok(invoice);
        }

        [HttpPost("reservation/{reservationId}")]
        public async Task<IActionResult> CreateInvoice(int reservationId)
        {
            try
            {
                var createdInvoice = await _invoiceService.CreateInvoiceAsync(reservationId);
                return CreatedAtAction(nameof(GetInvoiceById), new { id = createdInvoice.Id }, createdInvoice);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/additional-charges")]
        public async Task<IActionResult> UpdateInvoiceWithAdditionalCharges(int id, [FromQuery] decimal additionalCharges)
        {
            var result = await _invoiceService.UpdateInvoiceAsync(id, additionalCharges);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var result = await _invoiceService.DeleteInvoiceAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}