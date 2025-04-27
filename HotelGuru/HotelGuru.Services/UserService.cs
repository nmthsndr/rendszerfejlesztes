using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace HotelGuru.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto> RegisterUserAsync(UserRegisterDto userDto);
        Task<UserDto?> UpdateUserAsync(int id, UserUpdateDto userDto);
        Task<bool> DeleteUserAsync(int id);
        Task SeedRolesAsync(AppDbContext context);
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
            // Ellenőrizd, hogy a felhasználó már létezik-e
            if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            // Térképezd át a DTO-t az entitásra
            var user = _mapper.Map<User>(userDto);

            // Hash-eld a jelszót
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            // Kezeld a szerepköröket
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

            // Update roles if provided
            if (userDto.RoleIds != null)
            {
                user.Roles.Clear();
                var roles = await _context.Roles
                    .Where(r => userDto.RoleIds.Contains(r.Id))
                    .ToListAsync();
                user.Roles = roles;
            }

            await _context.SaveChangesAsync();

            // Reload with includes
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

        public string GenerateJwtToken(User user, string secretKey)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Email, user.Email)
    };

            // Add roles to claims
            if (user.Roles != null)
            {
                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
                }
            }

            var token = new JwtSecurityToken(
                issuer: "HotelGuru",
                audience: "HotelGuru",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task SeedRolesAsync(AppDbContext context)
        {
            var roles = new List<Role>
            {
                new Role { Name = "Vendég" },
                new Role { Name = "Felhasználó" },
                new Role { Name = "Recepciós" },
                new Role { Name = "Adminisztrátor" }
            };

            foreach (var role in roles)
            {
                if (!await context.Roles.AnyAsync(r => r.Name == role.Name))
                {
                    context.Roles.Add(role);
                }
            }

            await context.SaveChangesAsync();
        }


        public async Task<string> LoginAsync(string email, string password, string secretKey)
        {
            // Keresd meg a felhasználót az email alapján
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == email);

            // Ellenőrizd, hogy a felhasználó létezik-e, és a jelszó helyes-e
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Hibás email vagy jelszó.");
            }

            // Generálj JWT tokent
            return GenerateJwtToken(user, secretKey);
        }


        private bool VerifyPassword(string password, string storedHash)
        {
            // Implement password verification logic here
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

    }
}