using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelGuru.Services
{
    public interface IAdminService
    {
        Task<bool> UpdateRoomStatusAsync(int roomId, bool isAvailable);
        Task<bool> UpdateRoomDetailsAsync(int roomId, Room updatedRoom);
    }

    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;

        public AdminService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UpdateRoomStatusAsync(int roomId, bool isAvailable)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) return false;

            room.Avaible = isAvailable;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateRoomDetailsAsync(int roomId, Room updatedRoom)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) return false;

            room.Type = updatedRoom.Type;
            room.Price = updatedRoom.Price;
            room.Avaible = updatedRoom.Avaible;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
