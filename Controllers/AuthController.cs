using MeetingNow.Helpers;
using MeetingNow.Models;
using MeetingNow.Models.ReqestModels;
using MeetingNow.Models.ResponseModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace MeetingNow.Controllers
{
    [Route("api/{controller}")]
    public class AuthController : Controller
    {
        ApplicationContext appCtx;

        public AuthController(ApplicationContext ctx)
        {
            appCtx = ctx;
        }

        [HttpPost("authenticate")]
        public IActionResult Authentication([FromBody] UserModel user)
        {
            User u = appCtx.Users.FirstOrDefault(u => u.Login == user.Login && u.Password == user.Password);
            var identity = GetIdentity(u);
            if (identity == null)
            {
                return BadRequest();
            }

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromDays(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodeJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Json(new AuthResponse(u, encodeJwt));
        }

        private ClaimsIdentity GetIdentity(User user)
        {
            if(user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                    new Claim("UserId", user.UserId.ToString())
                };

                ClaimsIdentity claimsIdentity = 
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            return null;
        }
    }
}
