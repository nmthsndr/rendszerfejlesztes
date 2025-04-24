using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using HotelGuru.DataContext.Context;
using System;
using Microsoft.Extensions.DependencyInjection;
using HotelGuru.Data;
using HotelGuru.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<HotelGuruContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HotelGuruContext") ?? throw new InvalidOperationException("Connection string 'HotelGuruContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer("Server=(LocalDB)\\MSSQLLocalDB;Database=HotelGuruDB;Trusted_Connection=True;TrustServerCertificate=True;",
        b => b.MigrationsAssembly("HotelGuru.DataContext")); // Specify the migrations assembly
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<ReceptionistService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<HotelService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<AddressService>();
builder.Services.AddScoped<ExtraServiceService>();
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<InvoiceService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
