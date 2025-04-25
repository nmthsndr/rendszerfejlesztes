using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace HotelGuru.Services
{
    public interface IAdminService
    {
        Task<bool> UpdateRoomStatusAsync(int roomId, bool isAvailable);
        Task<RoomDto?> UpdateRoomDetailsAsync(int roomId, RoomUpdateDto roomDto);
        Task<IEnumerable<RoomDto>> GetAllRoomsWithStatusAsync();
        Task<bool> SetRoomMaintenanceAsync(int roomId, bool isUnderMaintenance);
    }

    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AdminService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> UpdateRoomStatusAsync(int roomId, bool isAvailable)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
                return false;

            room.Avaible = isAvailable;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<RoomDto?> UpdateRoomDetailsAsync(int roomId, RoomUpdateDto roomDto)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
                return null;

            _mapper.Map(roomDto, room);
            await _context.SaveChangesAsync();

            // Reload with includes to return complete data
            room = await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            return _mapper.Map<RoomDto>(room!);
        }

        public async Task<IEnumerable<RoomDto>> GetAllRoomsWithStatusAsync()
        {
            var rooms = await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.Reservation)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RoomDto>>(rooms);
        }

        public async Task<bool> SetRoomMaintenanceAsync(int roomId, bool isUnderMaintenance)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
                return false;

            // If room is under maintenance, it's not available
            room.Avaible = !isUnderMaintenance;

            // In a more complete implementation, we'd have a separate maintenance status field
            await _context.SaveChangesAsync();
            return true;
        }
    }
}