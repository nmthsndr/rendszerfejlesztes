using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelGuru.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomDto>> GetAllRoomsAsync();
        Task<RoomDto?> GetRoomByIdAsync(int id);
        Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(DateTime startDate, DateTime endDate);
        Task<RoomDto> CreateRoomAsync(RoomCreateDto roomDto, int hotelId);
        Task<RoomDto?> UpdateRoomAsync(int id, RoomUpdateDto roomDto);
        Task<bool> DeleteRoomAsync(int id);
    }

    public class RoomService : IRoomService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public RoomService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoomDto>> GetAllRoomsAsync()
        {
            var rooms = await _context.Rooms
                .Include(r => r.Hotel)
                .ToListAsync();
            return _mapper.Map<IEnumerable<RoomDto>>(rooms);
        }

        public async Task<RoomDto?> GetRoomByIdAsync(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
                return null;

            return _mapper.Map<RoomDto>(room);
        }

        public async Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
            {
                throw new ArgumentException("End date must be after start date");
            }

            // Get all rooms
            var allRooms = await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.Reservation)
                .ToListAsync();

            // Filter to available rooms for the specified dates
            var availableRooms = allRooms.Where(room =>
            {
                // Room is marked as available
                if (!room.Available)
                    return false;

                // Room has no reservation
                if (room.Reservation == null)
                    return true;

                // Room has a reservation but not for the requested dates
                return !(startDate < room.Reservation.EndDate && endDate > room.Reservation.StartDate);
            }).ToList();

            return _mapper.Map<IEnumerable<RoomDto>>(availableRooms);
        }

        public async Task<RoomDto> CreateRoomAsync(RoomCreateDto roomDto, int hotelId)
        {
            // Verify hotel exists
            var hotel = await _context.Hotels.FindAsync(hotelId);
            if (hotel == null)
            {
                throw new InvalidOperationException($"Hotel with ID {hotelId} not found");
            }

            var room = _mapper.Map<Room>(roomDto);

            // Set hotel and availability
            room.HotelId = hotelId;
            room.Available = true;

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            // Reload with includes
            room = await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.Id == room.Id);

            return _mapper.Map<RoomDto>(room);
        }

        public async Task<RoomDto?> UpdateRoomAsync(int id, RoomUpdateDto roomDto)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return null;

            // Update room properties
            if (!string.IsNullOrEmpty(roomDto.Type))
                room.Type = roomDto.Type;

            if (roomDto.Price.HasValue && roomDto.Price.Value > 0)
                room.Price = roomDto.Price.Value;

            // Only update availability if the room isn't currently reserved
            if (room.ReservationId == null)
                room.Available = roomDto.Available;

            await _context.SaveChangesAsync();

            // Reload with includes
            room = await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.Id == id);

            return _mapper.Map<RoomDto>(room);
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Reservation)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
                return false;

            // Don't allow deletion if room is currently reserved
            if (room.Reservation != null)
            {
                throw new InvalidOperationException("Cannot delete room that is currently reserved");
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}