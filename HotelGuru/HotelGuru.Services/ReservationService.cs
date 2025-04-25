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
                .Include(r => r.Invoice)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
        }

        public async Task<ReservationDto?> GetReservationByIdAsync(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Rooms)
                .Include(r => r.Invoice)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return null;

            return _mapper.Map<ReservationDto>(reservation);
        }

        public async Task<ReservationDto> CreateReservationAsync(ReservationCreateDto reservationDto, int userId)
        {
            var reservation = _mapper.Map<Reservation>(reservationDto);
            reservation.UserId = userId;

            // Validate dates
            if (reservation.StartDate >= reservation.EndDate)
            {
                throw new InvalidOperationException("End date must be after start date.");
            }

            // Check if all rooms are available for the dates
            // This is a simple implementation - in reality, you'd need more complex logic

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            // Reload with includes
            reservation = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Rooms)
                .Include(r => r.Invoice)
                .FirstOrDefaultAsync(r => r.Id == reservation.Id); 

            return _mapper.Map<ReservationDto>(reservation!);
        }

        public async Task<bool> CancelReservationAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return false;

            // Check if cancellation is allowed (e.g., 24 hours before start date)
            if (reservation.StartDate <= DateTime.Now.AddHours(24))
            {
                throw new InvalidOperationException("Cannot cancel reservation less than 24 hours before start date.");
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(int userId)
        {
            var reservations = await _context.Reservations
                .Where(r => r.UserId == userId)
                .Include(r => r.User)
                .Include(r => r.Rooms)
                .Include(r => r.Invoice)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
        }
    }
}