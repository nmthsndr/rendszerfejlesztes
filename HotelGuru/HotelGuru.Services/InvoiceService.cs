using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace HotelGuru.Services
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync();
        Task<InvoiceDto?> GetInvoiceByIdAsync(int id);
        Task<InvoiceDto?> GetInvoiceByReservationIdAsync(int reservationId);
        Task<InvoiceDto> CreateInvoiceAsync(int reservationId);
        Task<bool> UpdateInvoiceAsync(int id, decimal additionalCharges);
        Task<bool> DeleteInvoiceAsync(int id);
    }

    public class InvoiceService : IInvoiceService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public InvoiceService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Reservation)
                .ToListAsync();
            return _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
        }

        public async Task<InvoiceDto?> GetInvoiceByIdAsync(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Reservation)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return null;

            return _mapper.Map<InvoiceDto>(invoice);
        }

        public async Task<InvoiceDto?> GetInvoiceByReservationIdAsync(int reservationId)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Reservation)
                .FirstOrDefaultAsync(i => i.ReservationId == reservationId);

            if (invoice == null)
                return null;

            return _mapper.Map<InvoiceDto>(invoice);
        }

        public async Task<InvoiceDto> CreateInvoiceAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Rooms)
                    .ThenInclude(r => r.ExtraServices)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
                throw new InvalidOperationException("Reservation not found.");

            // Check if invoice already exists
            var existingInvoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.ReservationId == reservationId);

            if (existingInvoice != null)
                throw new InvalidOperationException("Invoice already exists for this reservation.");

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

        public async Task<bool> UpdateInvoiceAsync(int id, decimal additionalCharges)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
                return false;

            invoice.ExtraPrice += additionalCharges;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteInvoiceAsync(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
                return false;

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}