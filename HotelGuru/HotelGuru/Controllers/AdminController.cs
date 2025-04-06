using HotelGuru.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelGuru.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    { 
        private readonly IAdminService _adminService;
    }
}
