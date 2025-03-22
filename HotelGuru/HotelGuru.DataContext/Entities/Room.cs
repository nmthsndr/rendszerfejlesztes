using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelGuru.DataContext.Entities
{
    public class Room
    {
        public int RoomId { get; set; }
        public int RoomNumber { get; set; }
        public string Type { get; set; }
        public int Price { get; set; }
        public bool Avaible { get; set; }
        public int UserId { get; set; }
        public Reservation Reservation { get; set; }
        public List<ExtraService> extraServices { get; set; }
    }
}
