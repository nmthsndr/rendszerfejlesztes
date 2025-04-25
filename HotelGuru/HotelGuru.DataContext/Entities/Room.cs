using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelGuru.DataContext.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }

        public bool Available { get; set; }
        public int? UserId { get; set; }
        public int? ReservationId { get; set; }
        public Reservation? Reservation { get; set; }
        public List<ExtraService>? ExtraServices { get; set; }
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }
}