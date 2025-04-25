using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace HotelGuru.Services
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetAllAddressesAsync();
        Task<AddressDto?> GetAddressByIdAsync(int id);
        Task<IEnumerable<AddressDto>> GetAddressesByUserIdAsync(int userId);
        Task<AddressDto> CreateAddressAsync(AddressDto addressDto, int userId);
        Task<AddressDto?> UpdateAddressAsync(int id, AddressDto addressDto);
        Task<bool> DeleteAddressAsync(int id);
    }

    public class AddressService : IAddressService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AddressService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AddressDto>> GetAllAddressesAsync()
        {
            var addresses = await _context.Addresses
                .Include(a => a.User)
                .ToListAsync();
            return _mapper.Map<IEnumerable<AddressDto>>(addresses);
        }

        public async Task<AddressDto?> GetAddressByIdAsync(int id)
        {
            var address = await _context.Addresses
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (address == null)
                return null;

            return _mapper.Map<AddressDto>(address);
        }

        public async Task<IEnumerable<AddressDto>> GetAddressesByUserIdAsync(int userId)
        {
            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .Include(a => a.User)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AddressDto>>(addresses);
        }

        public async Task<AddressDto> CreateAddressAsync(AddressDto addressDto, int userId)
        {
            var address = _mapper.Map<Address>(addressDto);
            address.UserId = userId;

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return _mapper.Map<AddressDto>(address);
        }

        public async Task<AddressDto?> UpdateAddressAsync(int id, AddressDto addressDto)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
                return null;

            _mapper.Map(addressDto, address);
            await _context.SaveChangesAsync();

            return _mapper.Map<AddressDto>(address);
        }

        public async Task<bool> DeleteAddressAsync(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
                return false;

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}