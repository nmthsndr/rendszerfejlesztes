using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelGuru.Services
{
    public interface IInvoiceService
    {
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
        Task<Invoice?> GetInvoiceByIdAsync(int id);
        Task<Invoice> CreateInvoiceAsync(Invoice invoice);
        Task<bool> UpdateInvoiceAsync(int id, Invoice updatedInvoice);
        Task<bool> DeleteInvoiceAsync(int id);
    }

    public class InvoiceService : IInvoiceService
    {
        private readonly AppDbContext _context;

        public InvoiceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
        {
            return await _context.Invoices.ToListAsync();
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int id)
        {
            return await _context.Invoices.FindAsync(id);
        }

        public async Task<Invoice> CreateInvoiceAsync(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task<bool> UpdateInvoiceAsync(int id, Invoice updatedInvoice)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null) return false;

            invoice.Price = updatedInvoice.Price;
            invoice.ExtraPrice = updatedInvoice.ExtraPrice;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteInvoiceAsync(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null) return false;

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
