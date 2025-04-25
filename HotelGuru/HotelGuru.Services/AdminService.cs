using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelGuru.Services
{
    public interface IAdminService
    {
        Task<bool> UpdateRoomStatusAsync(int Id, bool isAvailable);
        Task<RoomDto?> UpdateRoomDetailsAsync(int Id, RoomUpdateDto roomDto);
        Task<IEnumerable<RoomDto>> GetAllRoomsWithStatusAsync();
        Task<bool> SetRoomMaintenanceAsync(int Id, bool isUnderMaintenance);
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

        public async Task<bool> UpdateRoomStatusAsync(int Id, bool isAvailable)
        {
            var room = await _context.Rooms.FindAsync(Id);
            if (room == null)
                return false;

            room.Available = isAvailable;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<RoomDto?> UpdateRoomDetailsAsync(int Id, RoomUpdateDto roomDto)
        {
            var room = await _context.Rooms.FindAsync(Id);
            if (room == null)
                return null;

            // Handle property updates
            if (!string.IsNullOrEmpty(roomDto.Type))
                room.Type = roomDto.Type;

            if (roomDto.Price.HasValue)
                room.Price = roomDto.Price.Value;

            room.Available = roomDto.Available;

            await _context.SaveChangesAsync();

            room = await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.Id == Id);

            return _mapper.Map<RoomDto>(room!);
        }

        public async Task<IEnumerable<RoomDto>> GetAllRoomsWithStatusAsync()
        {
            try
            {
                // Make sure to use proper Include statements
                var rooms = await _context.Rooms
                    .Include(r => r.Hotel)
                    .Include(r => r.Reservation)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<RoomDto>>(rooms);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Error in GetAllRoomsWithStatusAsync: {ex.Message}");
                throw; // Rethrow to let higher levels handle it
            }
        }

        public async Task<bool> SetRoomMaintenanceAsync(int Id, bool isUnderMaintenance)
        {
            var room = await _context.Rooms.FindAsync(Id);
            if (room == null)
                return false;

            room.Available = !isUnderMaintenance;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}