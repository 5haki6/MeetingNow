using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingNow.Models.ResponseModel
{
    public class AuthResponse
    {
        public int Id { get; set; }
        public string Login { get; set; }

        public string Token { get; set; }

        public AuthResponse(User user, string token)
        {
            Id = user.UserId;
            Login = user.Login;
            Token = token;
        }
    }
}
