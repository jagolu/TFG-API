using System;
using System.Linq;
using API.Areas.GroupManage.Models;
using API.Data;
using API.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.GroupManage.Controllers
{
    [Route("Group/[action]")]
    [ApiController]
    public class CreateGroupController : ControllerBase
    {
        private ApplicationDBContext _context;

        public CreateGroupController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("CreateGroup")]
        public IActionResult createGroup([FromBody] CreateGroup group )
        {
            var dbGroup = _context.Group.Where(g => g.name == group.name);

            User user = TokenUserManager.getUserFromToken(HttpContext, _context);

            if (dbGroup.Count() > 0) //If already exists a group with the same name
            {
                return BadRequest(new { error = "AlreadyExistingGroup" });
            }

            if(!canCreateANewGroup(group, user)) //The user cant create more groups
            {
                return BadRequest(new { error = "LimitationCreateGroup" });
            }

            Group newGroup = new Group { name = group.name, type = group.type };

            UserGroup userGroup = new UserGroup
            {
                User = user,
                Group = newGroup,
                role = _context.Role.Where(r => r.name == "GROUP_ADMIN").First(),
                dateRole = DateTime.Today
            };

            try
            {
                _context.Add(newGroup);
                _context.Add(userGroup);

                _context.SaveChanges();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            return Ok();
        }

        private bool canCreateANewGroup(CreateGroup group, User user)
        {
            int userGroups = 0;
            int limitationGroups = 0;
            _context.Entry(user).Reference("limitations").Load();

            if (group.type) //Official group
            {
                userGroups = user.groups.Where(g => g.Group.type).Count();
                limitationGroups = user.limitations.officialGroup;
            }
            else //Virtual group
            {
                userGroups = user.groups.Where(g => !g.Group.type).Count();
                limitationGroups = user.limitations.virtualGroup;
            }

            if (limitationGroups <= userGroups) //The user cant create a new group of the specificated type
            {
                return false;
            }

            return true;
        }
    }
}
