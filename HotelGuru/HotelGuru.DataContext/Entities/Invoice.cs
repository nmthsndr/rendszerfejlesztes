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
        public int Id { get; set; }
        public decimal Price { get; set; }
        public decimal ExtraPrice { get; set; }
        public decimal TotalPrice => Price + ExtraPrice;

        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;
    }

}
