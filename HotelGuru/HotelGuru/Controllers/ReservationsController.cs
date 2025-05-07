using Microsoft.AspNetCore.Mvc;
using HotelGuru.Services;
using HotelGuru.DataContext.Dtos;
using System.Security.Claims;

namespace HotelGuru.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReservations()
        {
            var reservations = await _reservationService.GetAllReservationsAsync();
            return Ok(reservations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationById(int id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            return Ok(reservation);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationCreateDto reservationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Get the user ID from the JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "User not authenticated or invalid user ID" });
                }

                var createdReservation = await _reservationService.CreateReservationAsync(reservationDto, userId);
                return CreatedAtAction(nameof(GetReservationById), new { id = createdReservation.Id }, createdReservation);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            try
            {
                var result = await _reservationService.CancelReservationAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserReservations(int userId)
        {
            var reservations = await _reservationService.GetUserReservationsAsync(userId);
            return Ok(reservations);
        }
    }
}