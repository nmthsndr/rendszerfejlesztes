using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace HotelGuru.Services
{
    public interface IReservationService
    {
        Task<IEnumerable<ReservationDto>> GetAllReservationsAsync();
        Task<ReservationDto?> GetReservationByIdAsync(int id);
        Task<ReservationDto> CreateReservationAsync(ReservationCreateDto reservationDto, int userId);
        Task<bool> CancelReservationAsync(int id);
        Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(int userId);
    }

    public class ReservationService : IReservationService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ReservationService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReservationDto>> GetAllReservationsAsync()
        {
            var reservations = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Rooms)
                    .ThenInclude(r => r.Hotel)
                .Include(r => r.Invoice)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
        }

        public async Task<ReservationDto?> GetReservationByIdAsync(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Rooms)
                    .ThenInclude(r => r.Hotel)
                .Include(r => r.Invoice)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return null;

            return _mapper.Map<ReservationDto>(reservation);
        }

        public async Task<ReservationDto> CreateReservationAsync(ReservationCreateDto reservationDto, int userId)
        {
            // Basic validation
            if (reservationDto.StartDate >= reservationDto.EndDate)
            {
                throw new InvalidOperationException("End date must be after start date.");
            }

            if (reservationDto.RoomIds == null || !reservationDto.RoomIds.Any())
            {
                throw new InvalidOperationException("At least one room must be selected.");
            }

            // Start transaction to ensure data consistency
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Map the DTO to entity
                var reservation = _mapper.Map<Reservation>(reservationDto);
                reservation.UserId = userId;

                // Get the selected rooms
                var roomIds = reservationDto.RoomIds;
                var rooms = await _context.Rooms
                    .Where(r => roomIds.Contains(r.Id))
                    .ToListAsync();

                if (rooms.Count != roomIds.Count)
                {
                    throw new InvalidOperationException("One or more selected rooms were not found.");
                }

                // Check room availability for the selected dates
                foreach (var room in rooms)
                {
                    var isRoomAvailable = await IsRoomAvailable(room.Id, reservationDto.StartDate, reservationDto.EndDate);
                    if (!isRoomAvailable)
                    {
                        throw new InvalidOperationException($"Room {room.Type} is not available for the selected dates.");
                    }
                }

                // Save the reservation
                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                // Associate rooms with the reservation
                foreach (var room in rooms)
                {
                    room.ReservationId = reservation.Id;
                    room.Available = false;
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Return the created reservation
                var createdReservation = await _context.Reservations
                    .Include(r => r.User)
                    .Include(r => r.Rooms)
                        .ThenInclude(r => r.Hotel)
                    .Include(r => r.Invoice)
                    .FirstOrDefaultAsync(r => r.Id == reservation.Id);

                return _mapper.Map<ReservationDto>(createdReservation!);
            }
            catch (Exception)
            {
                // Rollback transaction on error
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> CancelReservationAsync(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Rooms)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return false;

            // Check if cancellation is allowed
            if (reservation.StartDate <= DateTime.Now.AddHours(168))
            {
                throw new InvalidOperationException("Cannot cancel reservation less than a week before start date.");
            }

            // Start transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Mark associated rooms as available again
                foreach (var room in reservation.Rooms)
                {
                    room.ReservationId = null;
                    room.Available = true;
                }

                // Remove the reservation
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                // Rollback transaction on error
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(int userId)
        {
            var reservations = await _context.Reservations
                .Where(r => r.UserId == userId)
                .Include(r => r.User)
                .Include(r => r.Rooms)
                    .ThenInclude(r => r.Hotel)
                .Include(r => r.Invoice)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
        }

        private async Task<bool> IsRoomAvailable(int roomId, DateTime startDate, DateTime endDate)
        {
            // Check if the room exists and is available
            var room = await _context.Rooms
                .Include(r => r.Reservation)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null || !room.Available)
                return false;

            // If the room has no reservation, it's available
            if (room.Reservation == null)
                return true;

            // Check if there's any overlap with existing reservations
            var existingReservation = room.Reservation;

            // Check for date overlap
            if (startDate < existingReservation.EndDate && endDate > existingReservation.StartDate)
                return false;

            return true;
        }
    }
}