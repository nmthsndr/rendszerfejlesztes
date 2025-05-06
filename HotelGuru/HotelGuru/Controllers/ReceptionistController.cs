using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HotelGuru.Services;
using HotelGuru.DataContext.Dtos;

namespace HotelGuru.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "ReceptionistOnly")]  // Only users with Receptionist role can access
    public class ReceptionistController : ControllerBase
    {
        private readonly IReceptionistService _receptionistService;

        public ReceptionistController(IReceptionistService receptionistService)
        {
            _receptionistService = receptionistService;
        }

        [HttpGet("pending-reservations")]
        public async Task<IActionResult> GetPendingReservations()
        {
            var reservations = await _receptionistService.GetPendingReservationsAsync();
            return Ok(reservations);
        }

        [HttpPost("confirm-reservation/{reservationId}")]
        public async Task<IActionResult> ConfirmReservation(int reservationId)
        {
            try
            {
                var result = await _receptionistService.ConfirmReservationAsync(reservationId);
                if (!result)
                {
                    return NotFound("Reservation not found.");
                }
                return Ok(new { message = "Reservation confirmed successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("check-in/{reservationId}")]
        public async Task<IActionResult> CheckInGuest(int reservationId)
        {
            try
            {
                var result = await _receptionistService.CheckInGuestAsync(reservationId);
                if (!result)
                {
                    return NotFound("Reservation not found.");
                }
                return Ok(new { message = "Guest checked in successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("generate-invoice/{reservationId}")]
        public async Task<IActionResult> GenerateInvoice(int reservationId)
        {
            try
            {
                var invoice = await _receptionistService.GenerateInvoiceAsync(reservationId);
                if (invoice == null)
                {
                    return NotFound("Reservation not found.");
                }
                return Ok(invoice);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}