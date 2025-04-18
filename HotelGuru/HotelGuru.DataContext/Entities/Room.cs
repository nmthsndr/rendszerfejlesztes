﻿using System;
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
        public decimal Price { get; set; }
        public bool Avaible { get; set; }
        public Reservation Reservation { get; set; }
        public List<ExtraService> ExtraServices { get; set; }
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }
}
