using HotelGuru.DataContext.Context;
using HotelGuru.DataContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HotelGuru
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            // Check if there is any data already
            if (context.Users.Any())
            {
                return; // Database already has data
            }

            // Create roles
            var adminRole = new Role { Name = "Admin" };
            var receptionistRole = new Role { Name = "Receptionist" };
            var guestRole = new Role { Name = "Guest" };

            context.Roles.AddRange(adminRole,receptionistRole, guestRole);
            context.SaveChanges();

            // Create users with hashed passwords
            var users = new List<User>();

            // Admin user
            CreatePasswordHash("admin123", out byte[] adminPasswordHash, out byte[] adminPasswordSalt);
            var adminUser = new User
            {
                Name = "Admin User",
                Email = "admin@hotelguru.com",
                Phone = "+1234567890",
                PasswordHash = adminPasswordHash,
                PasswordSalt = adminPasswordSalt,
                Roles = new List<Role> { adminRole },
                Address = new List<Address>()
            };
            users.Add(adminUser);
       

            // Receptionist user
            CreatePasswordHash("receptionist123", out byte[] receptionistPasswordHash, out byte[] receptionistPasswordSalt);
            var receptionistUser = new User
            {
                Name = "Receptionist User",
                Email = "receptionist@hotelguru.com",
                Phone = "+1234567892",
                PasswordHash = receptionistPasswordHash,
                PasswordSalt = receptionistPasswordSalt,
                Roles = new List<Role> { receptionistRole },
                Address = new List<Address>()
            };
            users.Add(receptionistUser);

            // guest user
            CreatePasswordHash("guest123", out byte[] guestPasswordHash, out byte[] guestPasswordSalt);
            var guestUser = new User
            {
                Name = "Guest User",
                Email = "guest@example.com",
                Phone = "+1234567893",
                PasswordHash = guestPasswordHash,
                PasswordSalt = guestPasswordSalt,
                Roles = new List<Role> { guestRole },
                Address = new List<Address>()
            };
            users.Add(guestUser);

            context.Users.AddRange(users);
            context.SaveChanges();

            // Add addresses for users
            var addresses = new List<Address>
            {
                new Address
                {
                    ZipCode = 10001,
                    City = "New York",
                    Street = "5th Avenue",
                    HouseNumber = 123,
                    UserId = guestUser.Id
                },
                new Address
                {
                    ZipCode = 90210,
                    City = "Beverly Hills",
                    Street = "Rodeo Drive",
                    HouseNumber = 456,
                    UserId = adminUser.Id
                }
            };

            context.Addresses.AddRange(addresses);
            context.SaveChanges();

            // Create hotels
            var hotels = new List<Hotel>
            {
                new Hotel
                {
                    Name = "Luxury Palace",
                    Address = "123 Main Street, New York, NY 10001"
                },
                new Hotel
                {
                    Name = "Business Inn",
                    Address = "456 Market Street, San Francisco, CA 94105"
                },
                new Hotel
                {
                    Name = "Seaside Resort",
                    Address = "789 Ocean Drive, Miami, FL 33139"
                }
            };

            context.Hotels.AddRange(hotels);
            context.SaveChanges();

            // Create rooms
            var rooms = new List<Room>();

            // Rooms for Luxury Palace
            for (int i = 1; i <= 5; i++)
            {
                rooms.Add(new Room
                {
                    Type = "Standard Room",
                    Price = 150.00m,
                    Available = true,
                    HotelId = hotels[0].Id
                });
            }

            for (int i = 1; i <= 3; i++)
            {
                rooms.Add(new Room
                {
                    Type = "Deluxe Room",
                    Price = 250.00m,
                    Available = true,
                    HotelId = hotels[0].Id
                });
            }

            rooms.Add(new Room
            {
                Type = "Presidential Suite",
                Price = 500.00m,
                Available = true,
                HotelId = hotels[0].Id
            });

            // Rooms for Business Inn
            for (int i = 1; i <= 10; i++)
            {
                rooms.Add(new Room
                {
                    Type = "Business Room",
                    Price = 120.00m,
                    Available = true,
                    HotelId = hotels[1].Id
                });
            }

            rooms.Add(new Room
            {
                Type = "Executive Suite",
                Price = 230.00m,
                Available = true,
                HotelId = hotels[1].Id
            });

            // Rooms for Seaside Resort
            for (int i = 1; i <= 7; i++)
            {
                rooms.Add(new Room
                {
                    Type = "Ocean View Room",
                    Price = 190.00m,
                    Available = true,
                    HotelId = hotels[2].Id
                });
            }

            for (int i = 1; i <= 3; i++)
            {
                rooms.Add(new Room
                {
                    Type = "Beachfront Suite",
                    Price = 320.00m,
                    Available = true,
                    HotelId = hotels[2].Id
                });
            }

            context.Rooms.AddRange(rooms);
            context.SaveChanges();

            // Create extra services
            var extraServices = new List<ExtraService>
            {
                new ExtraService
                {
                    Name = "Room Service",
                    Description = "24/7 room service delivery",
                    Price = 15.00m
                },
                new ExtraService
                {
                    Name = "Spa Treatment",
                    Description = "1-hour relaxing spa treatment",
                    Price = 80.00m
                },
                new ExtraService
                {
                    Name = "Airport Shuttle",
                    Description = "Round-trip airport transportation",
                    Price = 50.00m
                },
                new ExtraService
                {
                    Name = "Breakfast Buffet",
                    Description = "Full breakfast buffet",
                    Price = 25.00m
                },
                new ExtraService
                {
                    Name = "Late Checkout",
                    Description = "Extended checkout until 3 PM",
                    Price = 30.00m
                }
            };

            context.ExtraServices.AddRange(extraServices);
            context.SaveChanges();

            // Create some reservations
            var startDate = DateTime.Now.AddDays(3);
            var endDate = startDate.AddDays(5);

            var reservation1 = new Reservation
            {
                StartDate = startDate,
                EndDate = endDate,
                PaymentMethod = "Credit Card",
                UserId = guestUser.Id,
                Rooms = new List<Room> { rooms[0] }
            };

            // Mark the rooms as unavailable
            rooms[0].Available = false;
            rooms[0].ReservationId = reservation1.Id;

            context.Reservations.Add(reservation1);
            context.SaveChanges();

            // Create an invoice for the reservation
            var totalDays = (reservation1.EndDate - reservation1.StartDate).Days;
            var roomPrice = reservation1.Rooms.Sum(r => r.Price * totalDays);

            var invoice1 = new Invoice
            {
                ReservationId = reservation1.Id,
                Price = roomPrice,
                ExtraPrice = 0 // No extra services yet
            };

            context.Invoices.Add(invoice1);
            context.SaveChanges();

            Console.WriteLine("Database initialization completed successfully!");
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}