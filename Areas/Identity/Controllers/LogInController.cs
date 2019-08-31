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
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The database context of the application</value>
        private ApplicationDBContext _context;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The database context</param>
        public LogInController(ApplicationDBContext context)
        {
            _context = context;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //
        
        [HttpPost]
        [AllowAnonymous]
        [ActionName("LogIn")]
        /// <summary>
        /// Logs an user in the application
        /// </summary>
        /// <param name="user">The info to log</param>
        /// See <see cref="Areas.Identity.Models.UserLogIn"/> to see the param structure
        /// <returns>The IActionResult of the login action</returns>
        /// See <see cref="Areas.Identity.Models.UserSession"/> to see the return structure
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
