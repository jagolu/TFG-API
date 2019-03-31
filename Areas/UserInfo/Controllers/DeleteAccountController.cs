using System;
using System.Linq;
using System.Net.Mail;
using API.Areas.UserInfo.Models;
using API.Data;
using API.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.UserInfo.Controllers
{
    [Route("User/[action]")]
    [ApiController]
    public class DeleteAccountController : ControllerBase
    {
        private ApplicationDBContext _context;

        public DeleteAccountController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("DeleteAccount")]
        public IActionResult getUser([FromBody] DeleteUser userDelete)
        {
            var authToken = HttpContext.Request?.Headers["Authorization"];
            string email = TokenGenerator.getEmailClaim(TokenGenerator.getBearerToken(authToken.Value));

            User user = _context.User.Where(u => u.email == email).First();

            _context.Entry(user).Reference("role").Load();

            if(user.password != PasswordHasher.hashPassword(user.password)) {
                return BadRequest(new { error = "CantDeleteAccount" });
            }

            bool beingAdmin = deleteAccountBeingAdmin(userDelete.email, user);
            bool beingNormal = deleteAccountBeingNormal(email, user);

            if(!beingAdmin && !beingNormal) {
                return BadRequest(new { error = "CantDeleteAccount" });
            }

            try {
                _context.SaveChanges();

            } catch (Exception) {
                return StatusCode(500);
            }

            return Ok();
        }

        private Boolean deleteAccountBeingAdmin(string email, User u)
        {
            if(u.role.name != "ADMIN") {
                return false;
            }

            if(!isValidEmail(email)){
                return false;
            }

            _context.User.Remove(
                _context.User.Where(
                    uu => uu.email == email
                ).First()
            );


            return true;
        }

        private Boolean deleteAccountBeingNormal(string email, User u)
        {
            if(u.role.name != "NORMAL_USER") {
                return false;
            }

            _context.User.Remove(
                _context.User.Where(
                    uu => uu.email == email
                ).First()
            );

            return true;
        }

        private bool isValidEmail(string email)
        {
            try {
                MailAddress m = new MailAddress(email);

                string e = _context.User.Where(u => u.email == email).First().email;

                return true;

            } catch (Exception) {
                return false;
            }
        }
    }
}
