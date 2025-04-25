using AutoMapper;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;

namespace HotelGuru.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User Mappings
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<UserRegisterDto, User>();
            CreateMap<UserUpdateDto, User>();

            // Address Mappings
            CreateMap<Address, AddressDto>().ReverseMap();

            // Room Mappings
            CreateMap<Room, RoomDto>();
            CreateMap<RoomCreateDto, Room>();
            CreateMap<RoomUpdateDto, Room>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RoomId));

            // Hotel Mappings
            CreateMap<Hotel, HotelDto>().ReverseMap();

            // Reservation Mappings
            CreateMap<Reservation, ReservationDto>()
                .ForMember(dest => dest.ReservationId, opt => opt.MapFrom(src => src.Id));
            CreateMap<ReservationCreateDto, Reservation>();

            // Invoice Mappings
            CreateMap<Invoice, InvoiceDto>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => (decimal)src.Price))
                .ForMember(dest => dest.ExtraPrice, opt => opt.MapFrom(src => (decimal)src.ExtraPrice));

            // ExtraService Mappings
            CreateMap<ExtraService, ExtraServiceDto>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => (decimal)src.Price));
            CreateMap<ExtraServiceDto, ExtraService>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => (int)src.Price));

            // Role Mappings
            CreateMap<Role, RoleDto>().ReverseMap();
        }
    }
}