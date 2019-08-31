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
    public class SignUpController : ControllerBase
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
        public SignUpController(ApplicationDBContext context)
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
        [ActionName("SignUp")]
        /// <summary>
        /// Signs up a user in the application
        /// </summary>
        /// <param name="user">The info to signs up a user</param>
        /// See <see cref="Areas.Identity.Models.UserSignUp"/> to know the param structure
        /// <returns>The IActionResult of the signup action</returns>
        public IActionResult signUp([FromBody] UserSignUp user)
        {
            var userExists = _context.User.Where(u => u.email == user.email);

            if (userExists.Count() != 0) {
                if(userExists.First().dateDeleted != null)
                {
                    return BadRequest(new { error = "DeleteRequested" });
                }

                return BadRequest(new { error = "EmailAlreadyExistsError" });
            }

            User newUser = new User {
                email = user.email,
                nickname = user.username,
                password = PasswordHasher.hashPassword(user.password),
                tokenValidation = (user.password == null) ? null : Guid.NewGuid().ToString("N"),
                role = RoleManager.getNormalUser(_context)
            };
            

            try { 
                _context.User.Add(newUser);
                _context.SaveChanges();

                Home.Util.GroupNew.launch(newUser, null, null, Home.Models.TypeGroupNew.WELCOME, false, _context);
                EmailSender.sendVerificationToken(newUser.email, newUser.nickname, newUser.tokenValidation);

            } catch (Exception) {

                return StatusCode(500);
            }

            return Ok();
        }
    }
}
