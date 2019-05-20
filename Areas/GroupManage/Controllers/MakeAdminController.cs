using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.GroupManage.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.GroupManage.Controllers
{
    [Route("Group/[action]")]
    [ApiController]
    public class MakeAdminController : ControllerBase
    {
        private ApplicationDBContext _context;

        public MakeAdminController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("MakeAdmin")]
        public IActionResult makeAdmin([FromBody] MakeAdmin order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to make admin to another user
            var groups = _context.Group.Where(g => g.name == order.groupName); //The group
            var targetUsers = _context.User.Where(u => u.publicId == order.publicId); //The user who will be the new admin

            // The group of the target user don't exist
            if (groups.Count() != 1 || targetUsers.Count() != 1)
            {
                return BadRequest(new { error = "" });
            }

            Group group = groups.First();
            User targetUser = targetUsers.First();

            _context.Entry(group).Collection("users").Load();
            List<UserGroup> members = group.users.Where(u => u.userId == user.id || u.userId == targetUser.id).ToList();

            //The users are not members of the group
            if (members.Count() != 2)
            {
                return BadRequest(new { error = "" });
            }

            if(!hasPermissions(members, user.id, order.makeAdmin))
            {
                return BadRequest(new { error = "" });
            }

            try
            {
                UserGroup nextAdmin = members.Where(m => m.userId != user.id).First();
                nextAdmin.role = _context.Role.Where(r => r.name == (order.makeAdmin ? "GROUP_ADMIN" : "GROUP_NORMAL")).First();
                _context.Update(nextAdmin);
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        private bool hasPermissions(List<UserGroup> members, Guid callerId, bool makeAdmin)
        {
            _context.Entry(members.First()).Reference("role").Load();
            _context.Entry(members.Last()).Reference("role").Load();
            string roleCaller = members.Where(m => m.userId == callerId).First().role.name;
            string rolenextAdmin = members.Where(m => m.userId != callerId).First().role.name;
            string role_groupMaker = _context.Role.Where(r => r.name == "GROUP_MAKER").First().name;
            string role_normal = _context.Role.Where(r => r.name == (makeAdmin ? "GROUP_NORMAL" : "GROUP_ADMIN")).First().name;

            if(rolenextAdmin != role_normal || roleCaller != role_groupMaker)
            {
                return false;
            }

            return true;
        }
    }
}
