using Microsoft.AspNetCore.Mvc;
using HotelGuru.Services;
using HotelGuru.DataContext.Entities;

namespace HotelGuru.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceptionistController : ControllerBase
    {
        private readonly IReceptionistService _receptionistService;

        public ReceptionistController(IReceptionistService receptionistService)
        {
            _receptionistService = receptionistService;
        }

        [HttpPost("ConfirmReservation")]
        public async Task<IActionResult> ConfirmReservation(int reservationId)
        {
            var result = await _receptionistService.ConfirmReservationAsync(reservationId);
            if (!result)
            {
                return NotFound("Reservation not found.");
            }
            return Ok("Reservation confirmed successfully.");
        }

        [HttpPost("CheckInGuest")]
        public async Task<IActionResult> CheckInGuest(int reservationId)
        {
            var result = await _receptionistService.CheckInGuestAsync(reservationId);
            if (!result)
            {
                return NotFound("Reservation not found.");
            }
            return Ok("Guest checked in successfully.");
        }

        [HttpPost("GenerateInvoice")]
        public async Task<IActionResult> GenerateInvoice(int reservationId)
        {
            var invoice = await _receptionistService.GenerateInvoiceAsync(reservationId);
            if (invoice == null)
            {
                return NotFound("Reservation not found.");
            }
            return Ok(invoice);
        }
    }
}
