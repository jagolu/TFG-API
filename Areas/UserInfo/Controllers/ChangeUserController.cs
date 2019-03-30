using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using API.Areas.UserInfo.Models;
using API.Data;
using API.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.UserInfo.Controllers
{

    [Route("User/[action]")]
    [ApiController]
    public class ChangeUserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ChangeUserController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("ChangeUserInfo")]
        public IActionResult changeUser([FromBody] ChangeUserInfo info)
        {
            var authToken = HttpContext.Request?.Headers["Authorization"];
            string email = TokenGenerator.getEmailClaim(TokenGenerator.getBearerToken(authToken.Value));

            User user = _context.User.Where(u => u.email == email).First();

            user.nickname = info.nickname ?? user.nickname;
            user.password = info.password ?? user.password ?? PasswordHasher.hashPassword(info.password);
            user.profileImg = info.image ?? user.profileImg;

            try {
                _context.Update(user);
                _context.SaveChanges();

            } catch (Exception) {
                return StatusCode(500);
            }

            return Ok();
        }
    }
}