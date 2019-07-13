using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.Admin.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Admin.Controllers
{
    [Route("Admin/[action]")]
    [ApiController]
    public class BanUserController : ControllerBase
    {
        private ApplicationDBContext _context;

        public BanUserController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("BanUser")]
        public IActionResult banUser([FromBody] BanUser order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            User targetUser = new User();
            if (!AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
                        
            if(!existUser(ref targetUser, order.publicUserId))
            {
                return BadRequest();
            }
            if(validOrder(targetUser, order.ban))
            {
                return BadRequest();
            }

            try
            {
                targetUser.open = !targetUser.open;
                _context.Update(targetUser);

                _context.SaveChanges();
                if(order.ban) manageGroups(targetUser);

                EmailSender.sendBanNotification(targetUser.email, targetUser.nickname, order.ban);

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest(500);
            }
        }

        private bool existUser(ref User user, string publicUserId)
        {
            List<User> existUser = _context.User.Where(u => u.publicId == publicUserId).ToList();
            if (existUser.Count() != 1)
            {
                return false;
            }

            user = existUser.First();

            return true;
        }

        private bool validOrder(User user, bool order)
        {
            bool userBlock = user.open;

            return userBlock != order;
        }

        private void manageGroups(User user)
        {
            _context.Entry(user).Collection("groups").Load();
            Role maker = RoleManager.getGroupMaker(_context);
            Role admin = RoleManager.getGroupAdmin(_context);
            Role normal = RoleManager.getGroupNormal(_context);
            user.groups.ToList().ForEach(g =>
            {
                List<UserGroup> members = GroupManage.Util.QuitUserFromGroup.getValidUsersInGroup(g, _context);
                _context.Entry(g).Reference("role").Load();

                if (members.Count() > 1)
                {
                    if(g.role == maker)
                    {
                        GroupManage.Util.QuitUserFromGroup.manageQuitMaker(members, maker, admin, normal, _context);
                    }

                    g.role = normal;
                    _context.Update(g);
                    _context.SaveChanges();
                }
                else
                {
                    _context.Entry(g).Reference("Group").Load();
                    GroupManage.Util.RemoveGroup.Remove(g.Group, _context);
                }
            });
        }
    }
}
