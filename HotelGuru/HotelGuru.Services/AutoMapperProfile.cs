using AutoMapper;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;

namespace HotelGuru.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() {
            // User Mappings
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<UserRegisterDto, User>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<Address, AddressDto>().ReverseMap();
        }
    }
}
