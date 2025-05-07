using HotelGuru.DataContext.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelGuru.DataContext.Dtos
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }

    public class ReservationCreateDto
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string PaymentMethod { get; set; } = string.Empty;
    }
}