using Microsoft.EntityFrameworkCore;
using HotelGuru.DataContext.Entities;

namespace HotelGuru.DataContext.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<ExtraService> ExtraServices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Beállítjuk a decimális mezők pontosságát
            modelBuilder.Entity<Room>()
                .Property(r => r.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Invoice>()
                .Property(i => i.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Invoice>()
                .Property(i => i.ExtraPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ExtraService>()
                .Property(e => e.Price)
                .HasColumnType("decimal(18,2)");

            // Beállítjuk a kapcsolatokat
            modelBuilder.Entity<Room>()
                .HasOne(r => r.Reservation)
                .WithMany(r => r.Rooms)
                .HasForeignKey(r => r.Id)
                .IsRequired(false)  // Opcionális kapcsolat
                .OnDelete(DeleteBehavior.SetNull);  // Törléskor null-ra állítjuk

            modelBuilder.Entity<ExtraService>()
                .HasOne(e => e.Invoice)
                .WithMany()
                .HasForeignKey(e => e.InvoiceId)
                .IsRequired(false)  // Opcionális kapcsolat
                .OnDelete(DeleteBehavior.SetNull);  // Törléskor null-ra állítjuk

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Reservation)
                .WithOne(r => r.Invoice)
                .HasForeignKey<Invoice>(i => i.ReservationId);

            // További kapcsolatok és beállítások...
        }
    }
}