using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelGuru.DataContext.Entities;

namespace HotelGuru.DataContext.Dtos
{
    public class RoomDto
    {
        public int RoomId { get; set; }
        public string Type { get; set; }
        public int Price { get; set; }
        public bool Avaible { get; set; }
        public int UserId { get; set; }
    }
}
