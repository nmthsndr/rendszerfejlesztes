using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace HotelGuru.Services
{
    public interface IReceptionistService
    {
        Task<IEnumerable<ReservationDto>> GetPendingReservationsAsync();
        Task<bool> ConfirmReservationAsync(int reservationId);
        Task<bool> CheckInGuestAsync(int reservationId);
        Task<InvoiceDto?> GenerateInvoiceAsync(int reservationId);
    }

    public class ReceptionistService : IReceptionistService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ReceptionistService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReservationDto>> GetPendingReservationsAsync()
        {
            var reservations = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Rooms)
                .Include(r => r.Invoice)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
        }

        public async Task<bool> ConfirmReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Rooms)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
                return false;

            // Check if rooms are available for the period
            foreach (var room in reservation.Rooms)
            {
                var conflicts = await _context.Reservations
                    .Where(r => r.Id != reservationId && r.Rooms.Any(rm => rm.Id == room.Id))
                    .Where(r =>
                        (reservation.StartDate >= r.StartDate && reservation.StartDate < r.EndDate) ||
                        (reservation.EndDate > r.StartDate && reservation.EndDate <= r.EndDate) ||
                        (reservation.StartDate <= r.StartDate && reservation.EndDate >= r.EndDate))
                    .AnyAsync();

                if (conflicts)
                {
                    throw new InvalidOperationException($"Room {room.Id} is not available for the selected dates.");
                }
            }

            // Mark reservation as confirmed            
            foreach (var room in reservation.Rooms)
            {
                room.Available = false; 
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckInGuestAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Rooms)
                .FirstOrDefaultAsync(r => r.Id == reservationId); 

            if (reservation == null)
                return false;

            // Check if reservation is for today
            if (reservation.StartDate.Date != DateTime.Today)
            {
                throw new InvalidOperationException("Cannot check in: reservation is not for today.");
            }

            // Update reservation status
            foreach (var room in reservation.Rooms)
            {
                room.Available = false;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<InvoiceDto?> GenerateInvoiceAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Rooms)
                    .ThenInclude(r => r.ExtraServices)
                .Include(r => r.Invoice)
                .FirstOrDefaultAsync(r => r.Id == reservationId); 

            if (reservation == null)
                return null;

            // Check if invoice already exists
            if (reservation.Invoice != null)
            {
                return _mapper.Map<InvoiceDto>(reservation.Invoice);
            }

            // Calculate total price
            var totalDays = (reservation.EndDate - reservation.StartDate).Days;
            if (totalDays <= 0) totalDays = 1; // Minimum 1 day

            var roomPrice = reservation.Rooms.Sum(r => r.Price * totalDays);
            var extraServicesPrice = reservation.Rooms
                .SelectMany(r => r.ExtraServices ?? new List<ExtraService>())
                .Sum(e => e.Price);

            var invoice = new Invoice
            {
                ReservationId = reservationId,
                Price = roomPrice, 
                ExtraPrice = extraServicesPrice 
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return _mapper.Map<InvoiceDto>(invoice);
        }
    }
}