using System;
using System.Linq;
using API.Areas.Identity.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Identity.Controllers
{
    [Route("Authorization/[action]")]
    [ApiController]
    public class RememberPasswordController : ControllerBase
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
        public RememberPasswordController(ApplicationDBContext context)
        {
            _context = context;
        }


        //
        // ─────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //
        
        [HttpPost]
        [AllowAnonymous]
        [ActionName("RememberPassword")]
        /// <summary>
        /// Send a email with the remember password token
        /// </summary>
        /// <param name="order">The info to remember the password</param>
        /// See <see cref="Areas.Identity.Models.RememberPassword"/> to know the param structure
        /// <returns>The IActionResult of the remember password action</returns>
        public IActionResult rememberPassword([FromBody] RememberPassword order)
        {
            var userExists = _context.User.Where(u => u.email == order.email);
            if (userExists.Count() != 1)
            {
                return BadRequest(new { error = "EmailDontExist"});
            }
            if((DateTime.Now - userExists.First().tokenP_expiresTime).Days < 1)
            {
                return BadRequest(new { error = "CantChangePasswordToday" });
            }

            try
            {
                User user = userExists.First();
                if (!user.open) return BadRequest(new { error = "YoureBanned" });
                if (user.tokenValidation != null) return BadRequest(new { error = "NotFullyRegister" });
                String token = Guid.NewGuid().ToString("N");
                user.tokenPassword = token;
                user.tokenP_expiresTime = DateTime.Now.AddDays(7);
                _context.Update(user);

                EmailSender.sendVerificationPasswordToken(user.email, user.nickname, token);

                _context.SaveChanges();

                return Ok(new { success = "SucessFullPasswordEmail" });
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
