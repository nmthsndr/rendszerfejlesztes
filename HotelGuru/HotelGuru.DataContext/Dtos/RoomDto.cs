using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelGuru.DataContext.Entities;

namespace HotelGuru.DataContext.Dtos
{
    public class RoomDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }
        public HotelDto Hotel { get; set; }
    }

    public class RoomCreateDto
    {
        [Required]
        public string Type { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        public bool Available { get; set; }
    }

    public class RoomUpdateDto
    {
        public int RoomId { get; set; }
        public string Type { get; set; }
        public decimal? Price { get; set; }
        public bool Available { get; set; }
    }
}