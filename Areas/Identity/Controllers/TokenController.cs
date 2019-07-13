using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.Identity.Models;
using API.Areas.Identity.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Identity.Controllers
{
    [Route("Authorization/[action]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private ApplicationDBContext _context;

        public TokenController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [AllowAnonymous]
        [ActionName("Refresh")]
        public IActionResult refresh([FromBody] RefreshRequest req)
        {
            if (TokenGenerator.isValidClaim(req.token)) return StatusCode(401);

            string email = TokenGenerator.getEmailClaim(req.token);
            string refreshToken = TokenGenerator.getRefreshTokenClaim(req.token);

            List<UserToken> savedRefreshToken = _context.UserToken.Where(ut => ut.refreshToken == refreshToken).ToList();

            if (savedRefreshToken.Count() != 1) {
                return StatusCode(401);
            }

            if(savedRefreshToken.First().expirationTime < DateTime.Now)
            {
                try
                {
                    _context.Remove(savedRefreshToken);
                    _context.SaveChanges();
                }
                catch (Exception) {}
                return StatusCode(401);
            }

            User user = _context.User.Where(u => u.email == email).First();
            if (!user.open) return BadRequest(new { error = "YoureBanned" });

            UserSession session = MakeUserSession.getUserSession(_context, user, req.provider);

            if (session == null) return StatusCode(500);

            return Ok(session);
        }
    }
}
