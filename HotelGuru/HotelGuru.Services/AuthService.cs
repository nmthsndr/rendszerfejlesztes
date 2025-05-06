using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HotelGuru.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(UserRegisterDto registerDto);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthService(AppDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
                throw new InvalidOperationException("Invalid email or password");

            // Verify password
            if (user.PasswordHash == null || user.PasswordSalt == null ||
                !VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
                throw new InvalidOperationException("Invalid email or password");

            // Generate token
            var token = _jwtService.GenerateToken(user, user.Roles);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Username = user.Name,
                Roles = user.Roles.Select(r => r.Name).ToList()
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(UserRegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                throw new InvalidOperationException("Email already exists");

            // Create password hash
            CreatePasswordHash(registerDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            // Create new user
            var user = new User
            {
                Name = registerDto.Username,
                Email = registerDto.Email,
                Phone = registerDto.Phone,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Roles = new List<Role>()
            };

            // Assign roles
            if (registerDto.RoleIds != null && registerDto.RoleIds.Any())
            {
                var roles = await _context.Roles
                    .Where(r => registerDto.RoleIds.Contains(r.Id))
                    .ToListAsync();
                user.Roles = roles;
            }
            else
            {
                // Assign default "Customer" role if no roles specified
                var defaultRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Name == "Customer");

                if (defaultRole == null)
                {
                    defaultRole = new Role { Name = "Customer" };
                    _context.Roles.Add(defaultRole);
                    await _context.SaveChangesAsync();
                }

                user.Roles = new List<Role> { defaultRole };
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate token
            var token = _jwtService.GenerateToken(user, user.Roles);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Username = user.Name,
                Roles = user.Roles.Select(r => r.Name).ToList()
            };
        }

        // Helper methods for password hashing
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                        return false;
                }
                return true;
            }
        }
    }
}