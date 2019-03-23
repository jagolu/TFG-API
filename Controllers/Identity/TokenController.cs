using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using API.Data;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace API.Controllers.Identity
{
    [Route("Authorization/[action]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private ApplicationDBContext _context;
        private IConfiguration _configuration;

        public TokenController(ApplicationDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        [ActionName("Refresh")]
        public IActionResult refresh([FromBody] refreshRequest req)
        {
            var principal = TokenGenerator.getPrincipalFromExpiredToken(req.token);

            if (principal == null) return BadRequest(new { error="InvalidToken" });

            string email = principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
            string refreshToken = principal.FindFirst("refreshToken").Value;
            var savedRefreshToken = _context.UserToken.Where(ut => ut.refreshToken == refreshToken);

            if (savedRefreshToken.Count() != 1) {
                return BadRequest(new { error = "InvalidToken" });
            }

            try {
                string newRefreshToken = TokenGenerator.generateRefreshToken(_context, email, req.provider);
                string newToken = TokenGenerator.generateToken(email, newRefreshToken);

                return Ok(new { token = newToken });

            } catch (Exception) {
                return BadRequest(new { error = "InvalidToken" });
            }
        }
    }


    public class refreshRequest
    {
        [Required]
        public string token { get; set; }

        public Boolean provider { get; set; } = false;
    }
}
