using MeetingNow.Models;
using MeetingNow.Models.ReqestModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingNow.Controllers
{
    [Route("api/{controller}")]
    public class RegistrationController : Controller
    {
        ApplicationContext appContext;
        public RegistrationController(ApplicationContext context)
        {
            appContext = context;
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserModel user)
        {
            if (appContext.Users.FirstOrDefault(u => u.Login == user.Login) == null)
            {
                User newUser = new User
                {
                    Login = user.Login,
                    Password = user.Password
                };

                appContext.Users.Add(newUser);
                appContext.SaveChanges();
                newUser = appContext.Users.FirstOrDefault(u => u.Login == newUser.Login);

                Profile profile = new Profile
                {
                    User = newUser,
                    UserId = newUser.UserId,
                    UserName = newUser.Login
                };
                appContext.Profiles.Add(profile);
                appContext.SaveChanges();

                return Ok();

            }

            return BadRequest(new { errorMessage = "This login is already used" });
            
        }

        

    }
}
