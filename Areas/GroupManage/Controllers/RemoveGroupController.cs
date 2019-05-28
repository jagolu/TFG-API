using System;
using System.Linq;
using API.Areas.GroupManage.Models;
using API.Areas.GroupManage.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.GroupManage.Controllers
{
    [Route("Group/[action]")]
    [ApiController]
    public class RemoveGroupController : ControllerBase
    {
        private ApplicationDBContext _context;

        public RemoveGroupController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("RemoveGroup")]
        public IActionResult removeGroup([FromBody] RemoveGroup order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to kick the user from the group
            UserGroup ugCaller = new UserGroup();
            Group group = new Group();

            if (!UserInGroup.checkUserInGroup(user.id, ref group, order.name, ref ugCaller, _context))
            {
                return BadRequest();
            }
            if (!hasPermissions(ugCaller, user))
            {
                return BadRequest();
            }
            if (!PasswordHasher.areEquals(order.userPassword, user.password))
            {
                return BadRequest(new { error = "IncorrectOldPassword" });
            }
            try
            {
                removeGroup(group);

                return Ok(new { success = "SuccesfullGroupRemoved" });
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        private bool hasPermissions(UserGroup ugCaller, User caller)
        {
            _context.Entry(ugCaller).Reference("role").Load();
            Role role_groupMaker = _context.Role.Where(r => r.name == "GROUP_MAKER").First();
            Role caller_role = ugCaller.role;

            if(role_groupMaker != caller_role)
            {
                return false;
            }

            return true;
        }

        private void removeGroup(Group group)
        {
            _context.Entry(group).Collection("users").Load();
            group.users.ToList().ForEach(user =>
            {
                _context.Remove(user);
            });

            _context.SaveChanges();
            _context.Remove(group);
            _context.SaveChanges();
        }
    }
}
