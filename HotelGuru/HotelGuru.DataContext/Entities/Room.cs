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
        public string Type { get; set; }
        public int Price { get; set; }
        public bool Avaible { get; set; }
        public int UserId { get; set; }
        public Reservation Reservation { get; set; }
<<<<<<< HEAD
        public List<ExtraService> ExtraServices { get; set; }
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
||||||| 0f11c71
        public List<ExtraService> ExtraServices { get; set; }
=======
>>>>>>> 748b57fa11528464f60244899601b0cd935f2f7e
    }
}
