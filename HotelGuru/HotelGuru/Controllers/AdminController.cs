using Microsoft.AspNetCore.Mvc;
using HotelGuru.Services;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;

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

        [HttpPost("UpdateRoomStatus")]
        public async Task<IActionResult> UpdateRoomStatus(int roomId, bool isAvailable)
        {
            var result = await _adminService.UpdateRoomStatusAsync(roomId, isAvailable);
            if (!result)
            {
                return NotFound("Room not found.");
            }
            return Ok("Room status updated successfully.");
        }

        [HttpPut("UpdateRoomDetails")]
        public async Task<IActionResult> UpdateRoomDetails(int roomId, [FromBody] RoomDto roomDto)
        {
            var updatedRoom = new Room
            {
                Id = roomDto.Id,
                Type = roomDto.Type,
                Price = roomDto.Price,
                Avaible = roomDto.Avaible,
                HotelId = roomDto.Hotel.Id
            };

            var result = await _adminService.UpdateRoomDetailsAsync(roomId, updatedRoom);
            if (!result)
            {
                return NotFound("Room not found.");
            }
            return Ok("Room details updated successfully.");
        }
    }
}
