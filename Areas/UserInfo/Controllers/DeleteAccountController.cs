﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Areas.Alive.Util;
using API.Areas.GroupManage.Util;
using API.Areas.UserInfo.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace API.Areas.UserInfo.Controllers
{
    [Route("User/[action]")]
    [ApiController]
    public class DeleteAccountController : ControllerBase
    {
        private ApplicationDBContext _context;
        private IHubContext<NotificationHub> _hub;

        public DeleteAccountController(ApplicationDBContext context, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        [HttpPost]
        [Authorize]
        [ActionName("DeleteAccount")]
        public async Task<IActionResult> deleteAccount([FromBody] DeleteUser userDelete)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
            string userDeletePass = PasswordHasher.hashPassword(userDelete.password);
            
            
            if(user.password != userDeletePass) {
                return BadRequest(new { error = "CantDeleteAccount" });
            }

            if(!await deleteAccountBeingNormal(user)) {
                return BadRequest(new { error = "CantDeleteAccount" });
            }

            try {
                user.dateDeleted = DateTime.Now;
                _context.SaveChanges();

            } catch (Exception){
                return StatusCode(500);
            }

            return Ok();
        }

        private async Task<bool> deleteAccountBeingNormal(User u)
        {
            _context.Entry(u).Reference("role").Load();
            if(u.role != RoleManager.getNormalUser(_context)) {
                return false;
            }

            if (!await removeGroups(u)){
                return false;
            }

            return true;
        }

        private async Task<bool> removeGroups(User user)
        {
            _context.Entry(user).Collection("groups").Load();

            List<UserGroup> groups = user.groups.ToList();


            for(int i = 0; i < groups.Count(); i++)
            {
                if (!await QuitUserFromGroup.quitUser(groups.ElementAt(i), _context, _hub))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
