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
        public decimal Price { get; set; }
        public decimal ExtraPrice { get; set; }
        public decimal TotalPrice => Price + ExtraPrice;
    }
}
