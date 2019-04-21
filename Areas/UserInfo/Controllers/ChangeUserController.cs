﻿using System;
using System.Text.RegularExpressions;
using API.Areas.UserInfo.Models;
using API.Data;
using API.Models;
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

            if (_changePass) return Ok(new { success = "PassChanged"});

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
            
            //The old password is not correct
            if (hashOldPassword != userActualPassword) {
                throw new Exception("IncorrectOldPassword");
            }

            //The both new password are not equal
            if (newPassword != repeatNewPassword) {
                throw new Exception("");
            }


            if(newPassword.Length<8 || newPassword.Length > 20 || !Regex.IsMatch(newPassword, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$")) {
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