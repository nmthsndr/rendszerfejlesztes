using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using HotelGuru.DataContext.Context;
using HotelGuru.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Use only AppDbContext, remove HotelGuruContext if it exists
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer("Server=(LocalDB)\\MSSQLLocalDB;Database=HotelGuruDB;Trusted_Connection=True;TrustServerCertificate=True;");
});

// Register services with their interfaces
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IReceptionistService, ReceptionistService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IExtraServiceService, ExtraServiceService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

// AutoMapper Config
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelGuru API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelGuru API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();