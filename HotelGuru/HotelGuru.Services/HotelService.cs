using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace HotelGuru.Services
{
    public interface IHotelService
    {
        Task<IEnumerable<HotelDto>> GetAllHotelsAsync();
        Task<HotelDto?> GetHotelByIdAsync(int id);
        Task<HotelDto> CreateHotelAsync(HotelDto hotelDto);
        Task<HotelDto?> UpdateHotelAsync(int id, HotelDto hotelDto);
        Task<bool> DeleteHotelAsync(int id);
    }

    public class HotelService : IHotelService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public HotelService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<HotelDto>> GetAllHotelsAsync()
        {
            var hotels = await _context.Hotels
                .Include(h => h.RoomList)
                .ToListAsync();
            return _mapper.Map<IEnumerable<HotelDto>>(hotels);
        }

        public async Task<HotelDto?> GetHotelByIdAsync(int id)
        {
            var hotel = await _context.Hotels
                .Include(h => h.RoomList)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hotel == null)
                return null;

            return _mapper.Map<HotelDto>(hotel);
        }

        public async Task<HotelDto> CreateHotelAsync(HotelDto hotelDto)
        {
            var hotel = _mapper.Map<Hotel>(hotelDto);

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();

            return _mapper.Map<HotelDto>(hotel);
        }

        public async Task<HotelDto?> UpdateHotelAsync(int id, HotelDto hotelDto)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
                return null;

            _mapper.Map(hotelDto, hotel);
            await _context.SaveChangesAsync();

            return _mapper.Map<HotelDto>(hotel);
        }

        public async Task<bool> DeleteHotelAsync(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
                return false;

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}