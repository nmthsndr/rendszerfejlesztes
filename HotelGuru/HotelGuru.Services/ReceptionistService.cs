using AutoMapper;
using Microsoft.EntityFrameworkCore;
using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;

namespace HotelGuru.Services
{
    public interface IReceptionistService
    {
        Task<bool> ConfirmReservationAsync(int reservationId);
        Task<bool> CheckInAsync(int reservationId);
        Task<bool> CreateInvoiceAsync(int reservationId); // Updated to match the return type
    }
    internal class ReceptionistService : IReceptionistService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ReceptionistService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<bool> ConfirmReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null)
            {
                throw new KeyNotFoundException("Reservation not found.");
            }

            foreach (var room in reservation.Rooms)
            {
                if (!room.Avaible)
                    return false;
            }

            return true;
        }

        public async Task<bool> CheckInAsync(int reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null)
            {
                throw new KeyNotFoundException("Reservation not found.");
            }

            foreach (var room in reservation.Rooms)
            {
                room.Avaible = false;
            }

            return true;
        }

        public async Task<bool> CreateInvoiceAsync(int reservationId) // Updated to match the interface
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null)
            {
                throw new KeyNotFoundException("Reservation not found.");
            }

            decimal roomsprice = 0;
            decimal extrasprice = 0;
            foreach (var room in reservation.Rooms)
            {
                roomsprice += room.Price;
                foreach (var extra in room.ExtraServices)
                {
                    extrasprice += extra.Price;
                }
            }

            var invoice = new Invoice
            {
                Price = roomsprice,
                ExtraPrice = extrasprice
            };

            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();

            return true; // Return true to indicate success
        }

        Task<bool> IReceptionistService.CreateInvoiceAsync(int reservationId)
        {
            throw new NotImplementedException();
        }
    }
}
