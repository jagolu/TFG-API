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
        private ApplicationDBContext _context;

        public ResetPasswordController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [AllowAnonymous]
        [ActionName("ResetPassword")]
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
