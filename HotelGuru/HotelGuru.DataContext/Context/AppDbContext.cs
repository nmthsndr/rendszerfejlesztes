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

            // Set precision for decimal fields
            modelBuilder.Entity<Room>()
                .Property(r => r.Price)
                .HasColumnType("decimal(18,2)");

            // Explicitly map the Available property to ensure it's correctly created in the database
            modelBuilder.Entity<Room>()
                .Property(r => r.Available)
                .HasColumnName("Available");

            modelBuilder.Entity<Invoice>()
                .Property(i => i.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Invoice>()
                .Property(i => i.ExtraPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ExtraService>()
                .Property(e => e.Price)
                .HasColumnType("decimal(18,2)");

            // Configure relationships
            modelBuilder.Entity<Room>()
                .HasOne(r => r.Reservation)
                .WithMany(r => r.Rooms)
                .HasForeignKey(r => r.ReservationId)  // Using ReservationId instead of Id as FK
                .IsRequired(false)  // Optional relationship
                .OnDelete(DeleteBehavior.SetNull);  // Set to null on delete

            modelBuilder.Entity<ExtraService>()
                .HasOne(e => e.Invoice)
                .WithMany()
                .HasForeignKey(e => e.InvoiceId)
                .IsRequired(false)  // Optional relationship
                .OnDelete(DeleteBehavior.SetNull);  // Set to null on delete

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Reservation)
                .WithOne(r => r.Invoice)
                .HasForeignKey<Invoice>(i => i.ReservationId);

            // In AppDbContext.cs, add to OnModelCreating method:

            // Set decimal field precision
            modelBuilder.Entity<Room>()
                .Property(r => r.Price)
                .HasColumnType("decimal(18,2)");

            // Map between entity property names and database column names
            modelBuilder.Entity<Room>()
                .Property(e => e.Id)
                .HasColumnName("Id");

            modelBuilder.Entity<Reservation>()
                .Property(e => e.Id)
                .HasColumnName("Id");

            // Additional relationships and configurations...
        }
    }
}