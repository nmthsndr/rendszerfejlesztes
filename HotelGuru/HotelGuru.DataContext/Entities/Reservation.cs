using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelGuru.DataContext.Entities
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;

        public List<Room> Rooms { get; set; } = new();

        public int UserId { get; set; }
        public User? User { get; set; }
        public int InvoiceId {  get; set; }
        public Invoice Invoice { get; set; } = null!; 
    }

}
