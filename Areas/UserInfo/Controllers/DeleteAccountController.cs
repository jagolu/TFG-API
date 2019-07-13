using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.GroupManage.Util;
using API.Areas.UserInfo.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
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
        public IActionResult deleteAccount([FromBody] DeleteUser userDelete)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
            string userDeletePass = PasswordHasher.hashPassword(userDelete.password);
            
            
            if(user.password != userDeletePass) {
                return BadRequest(new { error = "CantDeleteAccount" });
            }

            if(!deleteAccountBeingNormal(user)) {
                return BadRequest(new { error = "CantDeleteAccount" });
            }

            try {
                _context.User.Remove(user);
                _context.SaveChanges();

            } catch (Exception){
                return StatusCode(500);
            }

            return Ok();
        }

        private bool deleteAccountBeingNormal(User u)
        {
            _context.Entry(u).Reference("role").Load();
            if(u.role != RoleManager.getNormalUser(_context)) {
                return false;
            }

            if (!removeGroups(u)){
                return false;
            }

            return true;
        }

        private bool removeGroups(User user)
        {
            _context.Entry(user).Collection("groups").Load();

            List<UserGroup> groups = user.groups.ToList();


            for(int i = 0; i < groups.Count(); i++)
            {
                if (!QuitUserFromGroup.quitUser(groups.ElementAt(i), _context))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
