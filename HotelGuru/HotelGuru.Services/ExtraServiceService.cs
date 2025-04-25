using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace HotelGuru.Services
{
    public interface IExtraServiceService
    {
        Task<IEnumerable<ExtraServiceDto>> GetAllExtraServicesAsync();
        Task<ExtraServiceDto?> GetExtraServiceByIdAsync(int id);
        Task<ExtraServiceDto> CreateExtraServiceAsync(ExtraServiceDto extraServiceDto);
        Task<ExtraServiceDto?> UpdateExtraServiceAsync(int id, ExtraServiceDto extraServiceDto);
        Task<bool> DeleteExtraServiceAsync(int id);
    }

    public class ExtraServiceService : IExtraServiceService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ExtraServiceService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ExtraServiceDto>> GetAllExtraServicesAsync()
        {
            var extraServices = await _context.ExtraServices.ToListAsync();
            return _mapper.Map<IEnumerable<ExtraServiceDto>>(extraServices);
        }

        public async Task<ExtraServiceDto?> GetExtraServiceByIdAsync(int id)
        {
            var extraService = await _context.ExtraServices.FindAsync(id);
            if (extraService == null)
                return null;

            return _mapper.Map<ExtraServiceDto>(extraService);
        }

        public async Task<ExtraServiceDto> CreateExtraServiceAsync(ExtraServiceDto extraServiceDto)
        {
            var extraService = _mapper.Map<ExtraService>(extraServiceDto);

            _context.ExtraServices.Add(extraService);
            await _context.SaveChangesAsync();

            return _mapper.Map<ExtraServiceDto>(extraService);
        }

        public async Task<ExtraServiceDto?> UpdateExtraServiceAsync(int id, ExtraServiceDto extraServiceDto)
        {
            var extraService = await _context.ExtraServices.FindAsync(id);
            if (extraService == null)
                return null;

            _mapper.Map(extraServiceDto, extraService);
            await _context.SaveChangesAsync();

            return _mapper.Map<ExtraServiceDto>(extraService);
        }

        public async Task<bool> DeleteExtraServiceAsync(int id)
        {
            var extraService = await _context.ExtraServices.FindAsync(id);
            if (extraService == null)
                return false;

            _context.ExtraServices.Remove(extraService);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}