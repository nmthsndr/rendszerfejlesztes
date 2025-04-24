using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelGuru.Services
{
    public interface IReceptionistService
    {
        Task<IEnumerable<Reservation>> GetAllReservationsAsync();
        Task<Reservation?> GetReservationByIdAsync(int id);
        Task<Reservation> CreateReservationAsync(Reservation reservation);
        Task<bool> UpdateReservationAsync(int id, Reservation updatedReservation);
        Task<bool> DeleteReservationAsync(int id);
        Task<bool> ConfirmReservationAsync(int reservationId);
        Task<bool> CheckInGuestAsync(int reservationId);
        Task<Invoice?> GenerateInvoiceAsync(int reservationId);
    }


    public class ReceptionistService : IReceptionistService
    {
        private readonly AppDbContext _context;

        public ReceptionistService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
        {
            return await _context.Reservations.ToListAsync();
        }

        public async Task<Reservation?> GetReservationByIdAsync(int id)
        {
            return await _context.Reservations.FindAsync(id);
        }

        public async Task<Reservation> CreateReservationAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return reservation;
        }

        public async Task<bool> UpdateReservationAsync(int id, Reservation updatedReservation)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return false;

            reservation.StartDate = updatedReservation.StartDate;
            reservation.EndDate = updatedReservation.EndDate;
            reservation.PaymentMethod = updatedReservation.PaymentMethod;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReservationAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return false;

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConfirmReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null) return false;

            // Add logic to confirm the reservation (e.g., set a status)
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckInGuestAsync(int reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null) return false;

            // Add logic to check in the guest (e.g., update reservation status)
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Invoice?> GenerateInvoiceAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Rooms)
                .ThenInclude(r => r.ExtraServices)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null) return null;

            var invoice = new Invoice
            {
                ReservationId = reservationId,
                Price = (int)reservation.Rooms.Sum(r => r.Price), // Explicit cast to int
                ExtraPrice = reservation.Rooms.SelectMany(r => r.ExtraServices).Sum(e => e.Price),
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

    }
}
