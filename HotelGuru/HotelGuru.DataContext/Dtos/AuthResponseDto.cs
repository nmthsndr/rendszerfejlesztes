using System.Collections.Generic;

namespace HotelGuru.DataContext.Dtos
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public IList<string> Roles { get; set; }
    }
}