using System.Linq;
using API.Areas.Identity.Models;
using API.Data;
using API.Models;
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

            var savedRefreshToken = _context.UserToken.Where(ut => ut.refreshToken == refreshToken);

            if (savedRefreshToken.Count() != 1) {
                return StatusCode(401);
            }

            User user = _context.User.Where(u => u.email == email).First();

            UserSession session = UserSessionGenerator.getUserJson(_context, user, req.provider);

            if (session != null) {
                _context.SaveChanges();

                return Ok(session);
            }

            return StatusCode(401);
        }
    }
}
