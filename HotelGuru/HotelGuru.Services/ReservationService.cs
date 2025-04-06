using AutoMapper;
using Microsoft.EntityFrameworkCore;
using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Dtos;
using HotelGuru.DataContext.Entities;

namespace HotelGuru.Services
{
    public interface IReservationService
    {
        Task<ReservationDto> CreateReservationAsync(ReservationCreateDto reservationCreateDto, int userid, int invoiceid);
        Task<bool> CancelReservationAsync(int reservationId);
        Task<ReservationDto> GetReservationAsync(int reservationId);
    }
    public class ReservationService : IReservationService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ReservationService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<bool> CancelReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations.FirstOrDefaultAsync(o => o.ReservationId == reservationId);
            if (reservation == null)
                throw new KeyNotFoundException("Reservation not found.");
            var success= false;
            foreach (var room in reservation.Rooms)
            {
                if (!room.Avaible)
                {
                    success = true;
                    room.Avaible = true;
                    await _context.SaveChangesAsync();
                }
            }
            if(success) return true;
            return false;
        }

        public async Task<ReservationDto> CreateReservationAsync(ReservationCreateDto reservationCreateDto, int userid, int invoiceid)
        {
            var reservation = new Reservation
            {
                StartDate = reservationCreateDto.StartDate,
                EndDate = reservationCreateDto.EndDate,
                PaymentMethod = reservationCreateDto.PaymentMethod,
                Rooms=new List<Room>(),
                UserId= userid,
                InvoiceId=invoiceid
            };
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            return _mapper.Map<ReservationDto>(reservation);
        }

        public async Task<ReservationDto> GetReservationAsync(int reservationId)
        {
            /*var reservations=await _context.Reservations.Where(o=>o.ReservationId==reservationId)
                .Include(o=>o.Rooms).ThenInclude(r=>r.Reservation)*/
            throw new NotImplementedException();
                
        }
    }
}
