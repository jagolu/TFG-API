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
    public class LogInController : ControllerBase
    {
        private ApplicationDBContext _context;

        public LogInController(ApplicationDBContext context)
        {
            _context = context;
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

            UserSession session = UserSessionGenerator.getUserJson(_context, userExist.First(), user.provider);

            if (session != null) {
                _context.SaveChanges();

                return Ok( session );
            }

            return StatusCode(500);
        }
    }
}
