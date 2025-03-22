using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelGuru.DataContext.Entities
{
    public class Room
    {
        public int roomId { get; set; }
        public int roomNumber { get; set; }
        public string type { get; set; }
        public int price { get; set; }
        public bool avaible { get; set; }
    }
}
