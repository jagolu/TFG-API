using System;
using API.Areas.UserInfo.Models;
using API.Areas.UserInfo.Util;
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
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The database context of the application</value>
        private readonly ApplicationDBContext _context;

        /// <value>True if the user is doing a password change, false otherwise</value>
        private Boolean _changePass;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The context of the database</param>
        public ChangeUserController(ApplicationDBContext context)
        {
            _context = context;
            _changePass = false;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        [HttpPost]
        [Authorize]
        [ActionName("ChangeUserInfo")]
        /// <summary>
        /// Change the info of the user
        /// </summary>
        /// <param name="info">The new info of the user</param>
        /// See <see cref="Areas.UserInfo.Models.ChangeUserInfo"/> to know the param structure
        /// <returns>IActionResult of the change user info action</returns>
        /// See <see cref="Areas.UserInfo.Models.UserData"/> to know the response structure
        public IActionResult changeUser([FromBody] ChangeUserInfo info)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
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

            string successRes = "";
            if (_changePass) successRes = "PassChanged";

            UserData userShow = new UserData
            {
                email = user.email,
                nickname = user.nickname,
                img = user.profileImg,
                groups = GroupsOfUser.get(user, _context),
                timeSignUp = user.dateSignUp
            };

            return Ok(new { success = successRes, info = userShow});
        }


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Change the password of the user
        /// </summary>
        /// <param name="oldPassword">The old password of the user written by the user</param>
        /// <param name="newPassword">The new password of the user</param>
        /// <param name="userActualPassword">The real password of the user in the database</param>
        /// <returns>The new password of the user</returns>
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

        /// <summary>
        /// Change the username of the user
        /// </summary>
        /// <param name="newNickname">The new username of the user</param>
        /// <param name="userActualNickname">The actual username of the user in the database</param>
        /// <returns>The new username of the user</returns>
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