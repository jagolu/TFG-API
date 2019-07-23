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

            if( userExist.Count() != 1 || !PasswordHasher.areEquals(user.password, userExist.First().password))
            { 
                return BadRequest(new { error = "WrongEmailOrPassword" });
            }
            User loggedUser = userExist.First();
            if (loggedUser.tokenValidation != null)
            {
                return BadRequest(new { error = "NotValidatedYet" });
            }
            if(!loggedUser.open)
            {
                return BadRequest(new { error = "YoureBanned" });
            }
            if (loggedUser.dateDeleted != null)
            {
                ResetDelete.reset(loggedUser, _context);
                Home.Util.GroupNew.launch(loggedUser, null, null, Home.Models.TypeGroupNew.WELCOMEBACK, false, _context);
            }

            UserSession session = MakeUserSession.getUserSession(_context, userExist.First(), user.provider);

            if (session == null) return StatusCode(500);

            return Ok(session);
        }
    }
}
