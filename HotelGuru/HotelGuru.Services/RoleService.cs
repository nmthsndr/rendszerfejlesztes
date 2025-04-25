using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace HotelGuru.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto?> GetRoleByIdAsync(int id);
        Task<RoleDto?> GetRoleByNameAsync(string name);
        Task<RoleDto> CreateRoleAsync(RoleDto roleDto);
        Task<RoleDto?> UpdateRoleAsync(int id, RoleDto roleDto);
        Task<bool> DeleteRoleAsync(int id);
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(int roleId);
    }

    public class RoleService : IRoleService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public RoleService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _context.Roles
                .ToListAsync();
            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }

        public async Task<RoleDto?> GetRoleByIdAsync(int id)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return null;

            return _mapper.Map<RoleDto>(role);
        }

        public async Task<RoleDto?> GetRoleByNameAsync(string name)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower());

            if (role == null)
                return null;

            return _mapper.Map<RoleDto>(role);
        }

        public async Task<RoleDto> CreateRoleAsync(RoleDto roleDto)
        {
            // Check if role already exists
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name.ToLower() == roleDto.Name.ToLower());

            if (existingRole != null)
                throw new InvalidOperationException("Role with this name already exists.");

            var role = _mapper.Map<Role>(roleDto);

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return _mapper.Map<RoleDto>(role);
        }

        public async Task<RoleDto?> UpdateRoleAsync(int id, RoleDto roleDto)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return null;

            // Check if new name conflicts with existing role
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name.ToLower() == roleDto.Name.ToLower() && r.Id != id);

            if (existingRole != null)
                throw new InvalidOperationException("Role with this name already exists.");

            _mapper.Map(roleDto, role);
            await _context.SaveChangesAsync();

            return _mapper.Map<RoleDto>(role);
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            var role = await _context.Roles
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return false;

            if (role.Users.Any())
                throw new InvalidOperationException("Cannot delete role that is assigned to users.");

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(int roleId)
        {
            var role = await _context.Roles
                .Include(r => r.Users)
                    .ThenInclude(u => u.Address)
                .FirstOrDefaultAsync(r => r.Id == roleId);

            if (role == null)
                throw new InvalidOperationException("Role not found.");

            return _mapper.Map<IEnumerable<UserDto>>(role.Users);
        }
    }
}