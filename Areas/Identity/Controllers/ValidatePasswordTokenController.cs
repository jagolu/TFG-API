using System.ComponentModel.DataAnnotations;
using API.Areas.Identity.Util;
using API.Data;
using API.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Identity.Controllers
{
    [Route("Authorization/[action]")]
    [ApiController]
    public class ValidatePasswordTokenController : ControllerBase
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
        public ValidatePasswordTokenController(ApplicationDBContext context)
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
        [ActionName("checkPasswordToken")]
        /// <summary>
        /// Validate the password token to know if the user
        /// can change the password
        /// </summary>
        /// <param name="passwordToken">The password token</param>
        /// <returns>Status Code 200 if the password is valid, 400 otherwise</returns>
        public IActionResult checkPassword([Required] string passwordToken)
        {
            User u = new User();
            if (!ValidTokenPassword.isValid(passwordToken, ref u, _context))
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
