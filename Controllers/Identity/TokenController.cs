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
            if (TokenGenerator.isValidClaim(req.token)) return BadRequest(new { error="InvalidToken" });

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


    public class refreshRequest
    {
        [Required]
        public string token { get; set; }

        public Boolean provider { get; set; } = false;
    }
}
