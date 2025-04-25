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
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Roles))
                .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.Address))
                .ReverseMap();
            CreateMap<UserRegisterDto, User>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Username));
            CreateMap<UserUpdateDto, User>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Username));

            // Address Mappings
            CreateMap<Address, AddressDto>().ReverseMap();

            // Room Mappings
            CreateMap<Room, RoomDto>();
            CreateMap<RoomCreateDto, Room>();
            CreateMap<RoomUpdateDto, Room>();

            // Hotel Mappings
            CreateMap<Hotel, HotelDto>().ReverseMap();

            // Reservation Mappings
            CreateMap<Reservation, ReservationDto>();
            CreateMap<ReservationCreateDto, Reservation>();

            // Invoice Mappings
            CreateMap<Invoice, InvoiceDto>();
            CreateMap<InvoiceDto, Invoice>();

            // ExtraService Mappings
            CreateMap<ExtraService, ExtraServiceDto>();
            CreateMap<ExtraServiceDto, ExtraService>();

            // Role Mappings
            CreateMap<Role, RoleDto>().ReverseMap();
        }
    }
}