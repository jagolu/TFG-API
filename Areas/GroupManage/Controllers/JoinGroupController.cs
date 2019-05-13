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
        public IActionResult JoinGroup([FromBody] JoinGroup joinGroupInfo)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            _context.Entry(user).Collection("groups").Load();
            _context.Entry(user).Reference("limitations").Load();

            if(user.groups.Count() >= user.limitations.maxGroupJoins)
            {
                return BadRequest(new { error = "MaxGroupJoinsReached" });
            }

            //Group with the same name
            var dbGroup = _context.Group.Where(g => g.name == joinGroupInfo.groupName);

            if(dbGroup.Count() != 1)
            {
                return BadRequest(new { error = "" });
            }
            Group group = dbGroup.First();

            if (_context.UserGroup.Where(ug => ug.userId == user.id && ug.groupId == group.id).Count() != 0)
            {
                return BadRequest(new { error = "" });
            }

            if(group.password != null)
            {
                if(joinGroupInfo == null)
                {
                    return BadRequest(new { error = "" });
                }
                if (PasswordHasher.hashPassword(group.password) != joinGroupInfo.password)
                {
                    return BadRequest(new { error = "IncorrectPasswordJoiningGroup" });
                }
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
    }
}
