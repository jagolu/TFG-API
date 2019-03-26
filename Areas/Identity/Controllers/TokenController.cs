using System.Linq;
using API.Areas.Identity.Models;
using API.Data;
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
            if (TokenGenerator.isValidClaim(req.token)) return BadRequest(new { error = "InvalidToken" });

            string email = TokenGenerator.getEmailClaim(req.token);
            string refreshToken = TokenGenerator.getRefreshTokenClaim(req.token);

            var savedRefreshToken = _context.UserToken.Where(ut => ut.refreshToken == refreshToken);

            if (savedRefreshToken.Count() != 1) {
                return BadRequest(new { error = "InvalidToken" });
            }

            string nToken = TokenGenerator.generateTokenAndRefreshToken(_context, email, req.provider);

            if (nToken != null) return Ok(new { token = nToken });
            else return StatusCode(500);
        }
    }
}
