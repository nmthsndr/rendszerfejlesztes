using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelGuru.DataContext.Entities
{
    public class Invoice
    {
        public int Price { get; set; }
        public int ExtraPrice { get; set; }
        public int TotalPrice { get; set; }
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
    }
}
