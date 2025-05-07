using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace HotelGuru.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto> RegisterUserAsync(UserRegisterDto userDto);
        Task<UserDto?> UpdateUserAsync(int id, UserUpdateDto userDto);
        Task<bool> DeleteUserAsync(int id);
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.Address)
                .Include(u => u.Roles)
                .ToListAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Address)
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return null;

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> RegisterUserAsync(UserRegisterDto userDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            var user = _mapper.Map<User>(userDto);

            if (userDto.RoleIds != null && userDto.RoleIds.Any())
            {
                var roles = await _context.Roles
                    .Where(r => userDto.RoleIds.Contains(r.Id))
                    .ToListAsync();
                user.Roles = roles;
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            user = await _context.Users
                .Include(u => u.Address)
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            return _mapper.Map<UserDto>(user!);
        }

        public async Task<UserDto?> UpdateUserAsync(int id, UserUpdateDto userDto)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return null;

            _mapper.Map(userDto, user);

            if (userDto.RoleIds != null)
            {
                user.Roles.Clear();
                var roles = await _context.Roles
                    .Where(r => userDto.RoleIds.Contains(r.Id))
                    .ToListAsync();
                user.Roles = roles;
            }

            await _context.SaveChangesAsync();

            user = await _context.Users
                .Include(u => u.Address)
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);

            return _mapper.Map<UserDto>(user!);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}