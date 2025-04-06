using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelGuru.DataContext.Entities;

namespace HotelGuru.DataContext.Dtos
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public int ExtraPrice { get; set; }
        public int TotalPrice => Price + ExtraPrice;
    }
}
