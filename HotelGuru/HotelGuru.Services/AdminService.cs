using AutoMapper;
using Microsoft.EntityFrameworkCore;
using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;


namespace HotelGuru.Services
{
    public interface IAdminService
    {
        Task<bool> UpdateRoomAsync(int roomId, RoomDto roomDto);
        Task<bool> DeleteRoomAsync(int roomId);
        Task<IEnumerable<RoomDto>> GetAllRoomsAsync();
    }
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AdminService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<bool> DeleteRoomAsync(int roomId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RoomDto>> GetAllRoomsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRoomAsync(int roomId, RoomDto roomDto)
        {
            throw new NotImplementedException();
        }
    }
}
