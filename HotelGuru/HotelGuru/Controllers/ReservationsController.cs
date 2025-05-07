using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HotelGuru.Services;
using HotelGuru.DataContext.Dtos;
using System.Security.Claims;

namespace HotelGuru.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all endpoints
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        [Authorize(Policy = "StaffOnly")] // Only staff can view all reservations
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

            // Check if the user is authorized to view this reservation
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = int.Parse(userIdClaim.Value);
            var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);

            // Staff can view any reservation, users can only view their own
            if (!userRoles.Any(r => r == "Admin" || r == "Receptionist" || r == "Manager"))
            {
                // Simple authorization check - in a real app, you'd want to check if this reservation belongs to the user
                // This would require extending the DTO to include the user ID
                var userReservations = await _reservationService.GetUserReservationsAsync(userId);
                if (!userReservations.Any(r => r.Id == id))
                {
                    return Forbid();
                }
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
                // Get the user ID from the JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "User not authenticated or invalid user ID" });
                }

                // Staff can cancel any reservation, users can only cancel their own
                var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);
                if (!userRoles.Any(r => r == "Admin" || r == "Receptionist" || r == "Manager"))
                {
                    // Check if this reservation belongs to the user
                    var userReservations = await _reservationService.GetUserReservationsAsync(userId);
                    if (!userReservations.Any(r => r.Id == id))
                    {
                        return Forbid();
                    }
                }

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
            // Check if the requesting user is the same as the userId parameter or has staff role
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);
            var isStaff = userRoles.Any(r => r == "Admin" || r == "Receptionist" || r == "Manager");

            // Regular users can only see their own reservations
            if (!isStaff && int.Parse(userIdClaim.Value) != userId)
            {
                return Forbid();
            }

            var reservations = await _reservationService.GetUserReservationsAsync(userId);
            return Ok(reservations);
        }
    }
}