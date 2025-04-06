using AutoMapper;
using Microsoft.EntityFrameworkCore;
using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;


namespace HotelGuru.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomDto>> GetAllRoomsAsync();
        Task<RoomDto> GetRoomByIdAsync(int id);
        Task<RoomDto> CreateRoomAsync(RoomCreateDto foodDto);
        Task<RoomDto> UpdateRoomAsync(int id, RoomUpdateDto foodDto);
        Task<bool> DeleteRoomAsync(int id);
    }

    public class RoomService : IRoomService
    {
        public Task<RoomDto> CreateRoomAsync(RoomCreateDto foodDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteRoomAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RoomDto>> GetAllRoomsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<RoomDto> GetRoomByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<RoomDto> UpdateRoomAsync(int id, RoomUpdateDto foodDto)
        {
            throw new NotImplementedException();
        }
    }
}
