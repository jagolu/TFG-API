using System;
using System.Linq;
using API.Data;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Identity.Controllers
{
    [Route("Authorization/[action]")]
    [ApiController]
    public class LogOutController : ControllerBase
    {
        private ApplicationDBContext _context;

        public LogOutController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("LogOut")]
        public IActionResult logOut()
        {
            try {
                var authToken = HttpContext.Request?.Headers["Authorization"];
                string token = TokenGenerator.getBearerToken(authToken.Value);
                string refreshToken = TokenGenerator.getRefreshTokenClaim(token);

                _context.UserToken.Remove(
                    _context.UserToken.Where(ut => ut.refreshToken == refreshToken).First()
                );

                _context.SaveChanges();

            } catch(Exception) {
                return StatusCode(500);
            }
            return Ok();
        }
    }
}
