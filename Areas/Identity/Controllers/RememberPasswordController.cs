using System;
using System.ComponentModel.DataAnnotations;
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
        private ApplicationDBContext _context;

        public RememberPasswordController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [AllowAnonymous]
        [ActionName("RememberPassword")]
        public IActionResult rememberPassword([FromBody] RememberPassword order)
        {
            var userExists = _context.User.Where(u => u.email == order.email);
            if (userExists.Count() != 1)
            {
                return BadRequest(new { error = "EmailDontExist"});
            }
            if((DateTime.Now - userExists.First().tokenPassword_expirationTime).Days < 1)
            {
                return BadRequest(new { error = "CantChangePasswordToday" });
            }

            try
            {
                User user = userExists.First();
                if (!user.open) return BadRequest(new { error = "YoureBanned" });
                String token = Guid.NewGuid().ToString("N");
                user.tokenPassword = token;
                user.tokenPassword_expirationTime = DateTime.Now.AddDays(7);
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

        [HttpGet]
        [AllowAnonymous]
        [ActionName("checkPasswordToken")]
        public IActionResult checkPassword([Required] string passwordToken)
        {
            User u = new User();
            if (!isValidToken(passwordToken, ref u))
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [ActionName("ResetPassword")]
        public IActionResult resetPassword([FromBody] ResetPassword order)
        {
            User user = new User();
            if (!isValidToken(order.tokenPassword, ref user))
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
                user.tokenPassword_expirationTime = DateTime.Now;
                _context.Update(user);
                _context.SaveChanges();

                return Ok(new { success = "PassChanged"});
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }


        private bool isValidToken(string token, ref User user)
        {
            var tokenExists = _context.User.Where(u => u.tokenPassword == token);
            if (tokenExists.Count() != 1) //The token doesn't exists
            {
                return false;
            }

            //The token isn't valid
            if(DateTime.Now > tokenExists.First().tokenPassword_expirationTime)
            {
                return false;
            }
            user = tokenExists.First();

            return true;
        }
    }
}
