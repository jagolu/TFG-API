using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Areas.GroupManage.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.GroupManage.Controllers
{
    [Route("Group/[action]")]
    [ApiController]
    public class JoinGroupController : ControllerBase
    {
        private ApplicationDBContext _context;

        public JoinGroupController(ApplicationDBContext context)
        {
            _context = context;
        }


        [HttpPost]
        [Authorize]
        [ActionName("JoinGroup")]
        public IActionResult JoinGroup([FromBody] JoinGroup order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            Group group = new Group();

            if (!isUnderLimitations(user))
            {
                return BadRequest(new { error = "MaxGroupJoinsReached" });
            }
            if(!hasPermissions(user, ref group, order.groupName))
            {
                return BadRequest(new { error = "" });
            }

            if(group.password != null && !PasswordHasher.areEquals(order.password, group.password))
            {
                return BadRequest(new { error = "IncorrectPasswordJoiningGroup" });
            }

            try
            {
                UserGroup newUser = new UserGroup
                {
                    User = user,
                    Group = group,
                    role = _context.Role.Where(r => r.name == "GROUP_NORMAL").First(),
                    dateRole = DateTime.Today
                };

                _context.UserGroup.Add(newUser);
                _context.SaveChanges();

                return Ok(new { success="SuccesfullJoinGroup"});
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        private bool hasPermissions(User user, ref Group group, string groupName)
        {
            var dbGroup = _context.Group.Where(g => g.name == groupName);

            if (dbGroup.Count() != 1)
            {
                return false;
            }
            group = dbGroup.First();

            if (_context.UserGroup.Where(ug => ug.userId == user.id && ug.groupId == dbGroup.First().id).Count() != 0)
            {
                return false;
            }

            return true;
        }

        private bool isUnderLimitations(User user)
        {
            _context.Entry(user).Collection("groups").Load();
            _context.Entry(user).Reference("limitations").Load();

            if (user.groups.Where(g=> !g.blocked).ToList().Count() >= user.limitations.maxGroupJoins)
            {
                return false;
            }
            return true;
        }
    }
}
