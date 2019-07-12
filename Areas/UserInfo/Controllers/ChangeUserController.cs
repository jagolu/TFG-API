using System;
using API.Areas.UserInfo.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Areas.UserInfo.Controllers
{

    [Route("User/[action]")]
    [ApiController]
    public class ChangeUserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private Boolean _changePass;

        public ChangeUserController(ApplicationDBContext context)
        {
            _context = context;
            _changePass = false;
        }

        [HttpPost]
        [Authorize]
        [ActionName("ChangeUserInfo")]
        public IActionResult changeUser([FromBody] ChangeUserInfo info)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            bool isAdmin = AdminPolicy.isAdmin(user, _context);

            try {
                user.nickname = !isAdmin ? changeNickname(info.nickname, user.nickname) : user.nickname;
                user.password = changePassword(info.oldpassword, info.newPassword, user.password);
                user.profileImg = !isAdmin ? info.image ?? user.profileImg : user.profileImg;

                _context.Update(user);
                _context.SaveChanges();

            } catch (DbUpdateException) {
                return StatusCode(500);
            } catch (Exception e) {
                if (e.Message == "") return BadRequest();
                else return BadRequest(new { error = e.Message });
            }

            if (_changePass) return Ok(new { success = "PassChanged"});

            return Ok();
        }

        private string changePassword(string oldPassword, string newPassword, string userActualPassword)
        {
            if (newPassword == null) {
                return userActualPassword;
            }

            if (oldPassword == null && userActualPassword!=null) {
                return userActualPassword;
            }

            String hashOldPassword = (userActualPassword == null && oldPassword == "") ?
                                      null : PasswordHasher.hashPassword(oldPassword);
            
            //The old password is not correct
            if (hashOldPassword != userActualPassword) {
                throw new Exception("IncorrectOldPassword");
            }

            if (!PasswordHasher.validPassword(newPassword)){ 
                throw new Exception("");
            }

            _changePass = true;
            return PasswordHasher.hashPassword(newPassword);
        }

        private string changeNickname(string newNickname, string userActualNickname)
        {
            if(newNickname == null) {
                return userActualNickname;
            }

            if(newNickname.Length<3 || newNickname.Length > 20) {
                throw new Exception("");
            }

            return newNickname;
        }
    }
}