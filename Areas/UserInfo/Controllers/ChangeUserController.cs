using System;
using System.Linq;
using System.Text.RegularExpressions;
using API.Areas.UserInfo.Models;
using API.Data;
using API.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Areas.UserInfo.Controllers
{

    [Route("User/[action]")]
    [ApiController]
    public class ChangeUserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ChangeUserController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("ChangeUserInfo")]
        public IActionResult changeUser([FromBody] ChangeUserInfo info)
        {
            var authToken = HttpContext.Request?.Headers["Authorization"];
            string email = TokenGenerator.getEmailClaim(TokenGenerator.getBearerToken(authToken.Value));

            User user = _context.User.Where(u => u.email == email).First();

            try {
                user.nickname = changeNickname(info.nickname, user.nickname);
                user.password = changePassword(info.oldpassword, info.newPassword, info.repeatNewPassword, user.password);
                user.profileImg = info.image ?? user.profileImg;

                _context.Update(user);
                _context.SaveChanges();

            } catch (DbUpdateException) {
                return StatusCode(500);
            } catch (Exception e) {
                return BadRequest(new { error=e.Message });
            }

            return Ok();
        }

        private string changePassword(string oldPassword, string newPassword, string repeatNewPassword, string userActualPassword)
        {
            if (newPassword == null || repeatNewPassword == null) {
                return userActualPassword;
            }

            if (oldPassword == null && userActualPassword!=null) {
                return userActualPassword;
            }

            String hashOldPassword = (userActualPassword == null && oldPassword == "") ?
                                      null : PasswordHasher.hashPassword(oldPassword);
            
            if (hashOldPassword != userActualPassword) {
                return userActualPassword;
            }

            //Lanzar exception devolviendo un BadRequest de que las contraseñas no son iguales
            if (newPassword != repeatNewPassword) {
                throw new Exception("INVALIDCHANGEPASSWORD");
            }

            if(newPassword.Length<8 || newPassword.Length > 20 || !Regex.IsMatch(newPassword, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$")) {
                throw new Exception("INVALIDCHANGEPASSWORD");
            }

            return PasswordHasher.hashPassword(newPassword);
        }

        private string changeNickname(string newNickname, string userActualNickname)
        {
            if(newNickname == null) {
                return userActualNickname;
            }

            //Throw exception
            if(newNickname.Length<3 || newNickname.Length > 20) {
                throw new Exception("INVALIDCHANGENICKNAME");
            }

            return newNickname;
        }
    }
}