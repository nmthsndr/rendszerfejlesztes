using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelGuru.Services
{
    public interface IHotelService
    {
        Task<IEnumerable<Hotel>> GetAllHotelsAsync();
        Task<Hotel?> GetHotelByIdAsync(int id);
        Task<Hotel> CreateHotelAsync(Hotel hotel);
        Task<bool> UpdateHotelAsync(int id, Hotel updatedHotel);
        Task<bool> DeleteHotelAsync(int id);
    }

    public class HotelService : IHotelService
    {
        private readonly AppDbContext _context;

        public HotelService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Hotel>> GetAllHotelsAsync()
        {
            return await _context.Hotels.ToListAsync();
        }

        public async Task<Hotel?> GetHotelByIdAsync(int id)
        {
            return await _context.Hotels.FindAsync(id);
        }

        public async Task<Hotel> CreateHotelAsync(Hotel hotel)
        {
            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();
            return hotel;
        }

        public async Task<bool> UpdateHotelAsync(int id, Hotel updatedHotel)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return false;

            hotel.Name = updatedHotel.Name;
            hotel.Address = updatedHotel.Address;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteHotelAsync(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return false;

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

