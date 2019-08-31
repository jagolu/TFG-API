using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using API.Areas.Identity.Models;
using API.Areas.Identity.Util;
using API.Data;
using API.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Identity.Controllers
{
    [Route("Authorization/[action]")]
    [ApiController]
    public class ValidationEmailController : ControllerBase
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
        public ValidationEmailController(ApplicationDBContext context)
        {
            _context = context;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        [HttpGet]
        [AllowAnonymous]
        [ActionName("Validate")]
        /// <summary>
        /// Validate an email by the email token
        /// </summary>
        /// <param name="emailToken">The email token</param>
        /// <param name="provider">The provider of the caller</param>
        /// <returns>The IActionResult of the email validation</returns>
        /// See <see cref="Areas.Identity.Models.UserSession"/> to know the return structure
        public IActionResult validateEmail([Required] string emailToken, Boolean provider = false)
        {
            var tokenExists = _context.User.Where(u => u.tokenValidation == emailToken);

            if (tokenExists.Count() != 1) {
                return BadRequest();
            }

            User user = tokenExists.First();
            if (API.Util.AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");

            user.tokenValidation = null;

            _context.Update(user);

            UserSession session = MakeUserSession.getUserSession(_context, user, provider);

            if (session == null) return StatusCode(500);

            return Ok(session);
        }
    }
}
