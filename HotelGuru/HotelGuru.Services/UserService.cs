using AutoMapper;
using Microsoft.EntityFrameworkCore;
using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;

namespace HotelGuru.Services
{
    public interface IUserService
    {
        Task<UserDto> RegisterAsync(UserRegisterDto userDto);
        Task<string> LoginAsync(UserLoginDto userDto);
        Task<UserDto> UpdateProfileAsync(int userId, UserUpdateDto userDto);
        Task<UserDto> UpdateAddressAsync(int userId, AddressDto addressDto);
        Task<IList<RoleDto>> GetRolesAsync();
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserService(
            AppDbContext context,
            IMapper mapper
            )
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDto> RegisterAsync(UserRegisterDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            user.Roles = new List<Role>();

            if (userDto.RoleIds != null)
            {
                foreach (var roleId in userDto.RoleIds)
                {
                    var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
                    if (existingRole != null)
                    {
                        user.Roles.Add(existingRole);
                    }
                }
            }

            if (!user.Roles.Any())
            {
                user.Roles.Add(await GetDefaultCustomerRoleAsync());
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        private async Task<Role> GetDefaultCustomerRoleAsync()
        {
            var customerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Customer");
            if (customerRole == null)
            {
                customerRole = new Role { Name = "Customer" };
                await _context.Roles.AddAsync(customerRole);
                await _context.SaveChangesAsync();
            }
            return customerRole;
        }

        public async Task<string> LoginAsync(UserLoginDto userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userDto.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }
            return user.Name;
        }

        public async Task<UserDto> UpdateProfileAsync(int userId, UserUpdateDto userDto)
        {
            var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            _mapper.Map(userDto, user);

            if (userDto.RoleIds != null && userDto.RoleIds.Any())
            {
                user.Roles.Clear();

                foreach (var roleId in userDto.RoleIds)
                {
                    var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
                    if (existingRole != null)
                    {
                        user.Roles.Add(existingRole);
                    }
                }
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateAddressAsync(int userId, AddressDto addressDto)
        {
            var user = await _context.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var address = _mapper.Map<Address>(addressDto);
            user.Address.Add(address);

            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<IList<RoleDto>> GetRolesAsync()
        {
            var roles = await _context.Roles.ToListAsync();
            return _mapper.Map<IList<RoleDto>>(roles);
        }
    }
}
