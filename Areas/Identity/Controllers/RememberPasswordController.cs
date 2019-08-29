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
