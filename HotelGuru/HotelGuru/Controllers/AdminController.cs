using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HotelGuru.Services;
using HotelGuru.DataContext.Dtos;

namespace HotelGuru.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]  // Only users with Admin role can access
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("rooms")]
        public async Task<IActionResult> GetAllRoomsWithStatus()
        {
            var rooms = await _adminService.GetAllRoomsWithStatusAsync();
            return Ok(rooms);
        }

        [HttpPost("rooms/{Id}/status")]
        public async Task<IActionResult> UpdateRoomStatus(int Id, [FromQuery] bool isAvailable)
        {
            var result = await _adminService.UpdateRoomStatusAsync(Id, isAvailable);
            if (!result)
            {
                return NotFound("Room not found.");
            }
            return Ok(new { message = "Room status updated successfully." });
        }

        [HttpPut("rooms/{Id}")]
        public async Task<IActionResult> UpdateRoomDetails(int Id, [FromBody] RoomUpdateDto roomDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedRoom = await _adminService.UpdateRoomDetailsAsync(Id, roomDto);
            if (updatedRoom == null)
            {
                return NotFound("Room not found.");
            }
            return Ok(updatedRoom);
        }

        [HttpPost("rooms/{Id}/maintenance")]
        public async Task<IActionResult> SetRoomMaintenance(int Id, [FromQuery] bool isUnderMaintenance)
        {
            var result = await _adminService.SetRoomMaintenanceAsync(Id, isUnderMaintenance);
            if (!result)
            {
                return NotFound("Room not found.");
            }
            return Ok(new { message = $"Room maintenance status set to {isUnderMaintenance}." });
        }
    }
}