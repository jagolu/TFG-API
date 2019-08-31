using System;
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
    public class ResetPasswordController : ControllerBase
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
        public ResetPasswordController(ApplicationDBContext context)
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
        [ActionName("ResetPassword")]
        /// <summary>
        /// Change the password of a user when the user uses the "remember password option"
        /// </summary>
        /// <param name="order">The info of the reset password</param>
        /// See <see cref="Areas.Identity.Models.ResetPassword"/> to know the param structure
        /// <returns></returns>
        public IActionResult reset([FromBody] ResetPassword order)
        {
            User user = new User();
            if (!ValidTokenPassword.isValid(order.tokenPassword, ref user, _context))
            {
                return BadRequest();
            }
            if (!PasswordHasher.validPassword(order.password))
            {
                return BadRequest();
            }

            try
            {
                user.password = PasswordHasher.hashPassword(order.password);
                user.tokenPassword = null;
                user.tokenP_expiresTime = DateTime.Now;
                _context.Update(user);
                _context.SaveChanges();

                return Ok(new { success = "PassChanged" });
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
