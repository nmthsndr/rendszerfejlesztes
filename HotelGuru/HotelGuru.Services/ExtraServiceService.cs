using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelGuru.Services
{
    public interface IExtraServiceService
    {
        Task<IEnumerable<ExtraService>> GetAllExtraServicesAsync();
        Task<ExtraService?> GetExtraServiceByIdAsync(int id);
        Task<ExtraService> CreateExtraServiceAsync(ExtraService extraService);
        Task<bool> UpdateExtraServiceAsync(int id, ExtraService updatedExtraService);
        Task<bool> DeleteExtraServiceAsync(int id);
    }
    public class ExtraServiceService : IExtraServiceService
    {
        private readonly AppDbContext _context;

        public ExtraServiceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExtraService>> GetAllExtraServicesAsync()
        {
            return await _context.ExtraServices.ToListAsync();
        }

        public async Task<ExtraService?> GetExtraServiceByIdAsync(int id)
        {
            return await _context.ExtraServices.FindAsync(id);
        }

        public async Task<ExtraService> CreateExtraServiceAsync(ExtraService extraService)
        {
            _context.ExtraServices.Add(extraService);
            await _context.SaveChangesAsync();
            return extraService;
        }

        public async Task<bool> UpdateExtraServiceAsync(int id, ExtraService updatedExtraService)
        {
            var extraService = await _context.ExtraServices.FindAsync(id);
            if (extraService == null) return false;

            extraService.Name = updatedExtraService.Name;
            extraService.Description = updatedExtraService.Description;
            extraService.Price = updatedExtraService.Price;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteExtraServiceAsync(int id)
        {
            var extraService = await _context.ExtraServices.FindAsync(id);
            if (extraService == null) return false;

            _context.ExtraServices.Remove(extraService);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
