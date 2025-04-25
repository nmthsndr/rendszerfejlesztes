using Microsoft.AspNetCore.Mvc;
using HotelGuru.Services;
using HotelGuru.DataContext.Dtos;

namespace HotelGuru.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpPost("rooms/{roomId}/status")]
        public async Task<IActionResult> UpdateRoomStatus(int roomId, [FromQuery] bool isAvailable)
        {
            var result = await _adminService.UpdateRoomStatusAsync(roomId, isAvailable);
            if (!result)
            {
                return NotFound("Room not found.");
            }
            return Ok(new { message = "Room status updated successfully." });
        }

        [HttpPut("rooms/{roomId}")]
        public async Task<IActionResult> UpdateRoomDetails(int roomId, [FromBody] RoomUpdateDto roomDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedRoom = await _adminService.UpdateRoomDetailsAsync(roomId, roomDto);
            if (updatedRoom == null)
            {
                return NotFound("Room not found.");
            }
            return Ok(updatedRoom);
        }

        [HttpPost("rooms/{roomId}/maintenance")]
        public async Task<IActionResult> SetRoomMaintenance(int roomId, [FromQuery] bool isUnderMaintenance)
        {
            var result = await _adminService.SetRoomMaintenanceAsync(roomId, isUnderMaintenance);
            if (!result)
            {
                return NotFound("Room not found.");
            }
            return Ok(new { message = $"Room maintenance status set to {isUnderMaintenance}." });
        }
    }
}