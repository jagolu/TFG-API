using System.Linq;
using API.Areas.Identity.Models;
using API.Data;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace API.Areas.Identity.Controllers
{
    [Route("Authorization/[action]")]
    [ApiController]
    public class LogInController : ControllerBase
    {
        private ApplicationDBContext _context;
        private IConfiguration _configuration;

        public LogInController(ApplicationDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        [HttpPost]
        [AllowAnonymous]
        [ActionName("LogIn")]
        public IActionResult logIn([FromBody] UserLogIn user)
        {
            var userExist = this._context.User.Where(u => u.email == user.email);

            if ((userExist.Count() != 1) || (PasswordHasher.hashPassword(user.password) != userExist.First().password)) {
                return BadRequest(new { error = "WrongEmailOrPassword" });
            }

            if ((userExist.First().tokenValidation != null)) {
                return BadRequest(new { error = "NotValidatedYet" });
            }

            string nToken = TokenGenerator.generateTokenAndRefreshToken(_context, user.email, user.provider);

            if (nToken != null) return Ok(new { token = nToken });
            else return StatusCode(500);
        }
    }
}
