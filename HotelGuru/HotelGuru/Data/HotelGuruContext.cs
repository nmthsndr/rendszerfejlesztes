using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HotelGuru.DataContext.Entities;

namespace HotelGuru.Data
{
    public class HotelGuruContext : DbContext
    {
        public HotelGuruContext (DbContextOptions<HotelGuruContext> options)
            : base(options)
        {
        }

        public DbSet<HotelGuru.DataContext.Entities.User> User { get; set; } = default!;
    }
}
