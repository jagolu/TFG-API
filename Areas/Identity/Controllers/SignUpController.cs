using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Areas.Identity.Models;
using API.Data;
using API.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Identity.Controllers
{
    [Route("Authorization/[action]")]
    [ApiController]
    public class SignUpController : ControllerBase
    {
        private ApplicationDBContext _context;

        public SignUpController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [AllowAnonymous]
        [ActionName("SignUp")]
        public IActionResult signUp([FromBody] UserSignUp user)
        {
            //if (!ModelState.IsValid) {
            //    return BadRequest(ModelState);
            //}
            var userExists = _context.User.Where(u => u.email == user.email).Count() != 0;

            if (userExists) {
                return BadRequest(new { error = "EmailAlreadyExistsError" });
            }

            User newUser = new User {
                email = user.email,
                nickname = user.username,
                password = PasswordHasher.hashPassword(user.password),
                tokenValidation = (user.password == null) ? null : Guid.NewGuid().ToString("N")
            };

            _context.User.Add(newUser);

            try {

                EmailSender.sendVerificationToken(newUser.email, newUser.nickname, newUser.tokenValidation);

                _context.SaveChanges();

            } catch (Exception) {

                return StatusCode(500);
            }

            return Ok();
        }
    }
}
