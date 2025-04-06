using HotelGuru.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelGuru.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceptionistController : ControllerBase
    {
        private readonly IReceptionistService _receptionService;
    }
}
