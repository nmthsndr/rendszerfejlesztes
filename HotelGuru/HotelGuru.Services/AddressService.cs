using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelGuru.Services
{
    public interface IAddressService
    {
        Task<IEnumerable<Address>> GetAllAddressesAsync();
        Task<Address?> GetAddressByIdAsync(int id);
        Task<Address> CreateAddressAsync(Address address);
        Task<bool> UpdateAddressAsync(int id, Address updatedAddress);
        Task<bool> DeleteAddressAsync(int id);
    }

    public class AddressService : IAddressService
    {
        private readonly AppDbContext _context;

        public AddressService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Address>> GetAllAddressesAsync()
        {
            return await _context.Addresses.ToListAsync();
        }

        public async Task<Address?> GetAddressByIdAsync(int id)
        {
            return await _context.Addresses.FindAsync(id);
        }

        public async Task<Address> CreateAddressAsync(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<bool> UpdateAddressAsync(int id, Address updatedAddress)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null) return false;

            address.City = updatedAddress.City;
            address.Street = updatedAddress.Street;
            address.ZipCode = updatedAddress.ZipCode;
            address.HouseNumber = updatedAddress.HouseNumber;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAddressAsync(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null) return false;

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
