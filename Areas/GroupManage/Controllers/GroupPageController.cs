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
    public class GroupPageController : ControllerBase
    {
        private ApplicationDBContext _context;

        public GroupPageController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("GroupPage")]
        public IActionResult getGroupPage(string groupName)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);

            var groups = _context.Group.Where(g => g.name == groupName);

            // If the group doesn't exist
            if (groups.Count() != 1)
            {
                return BadRequest(new { error = "" });
            }

            Group group = groups.First();
            _context.Entry(group).Collection("users").Load();

            // If the user doesn't belong to the group
            if (group.users.Where(u=>u.userId == user.id).Count() != 1)
            {
                return BadRequest(new { error = "" });
            }

            //Set the group name and group type
            GroupPage page = new GroupPage();
            page.groupName = group.name;
            page.groupType = group.type;

            //Get the role of the user in the group
            UserGroup ownUserGroup =  group.users.Where(u => u.userId == user.id).First();
            _context.Entry(ownUserGroup).Reference("role").Load();
            page.role = ownUserGroup.role.name;


            //Change ---- this is for try
            page.bets = new List<GroupBet>{
                new GroupBet { betName = "bet1", betBody = "betBody1"},
                new GroupBet { betName = "bet2", betBody = "betBody2"},
                new GroupBet { betName = "bet3", betBody = "betBody3"},
                new GroupBet { betName = "bet4", betBody = "betBody4"},
                new GroupBet { betName = "bet5", betBody = "betBody5"}
            };

            // Set the users who belongs to the group
            List<GroupMember> members = new List<GroupMember>();

            foreach(UserGroup ug in group.users)
            {
                _context.Entry(ug).Reference("role").Load();
                _context.Entry(ug).Reference("User").Load();
                members.Add(new GroupMember { userName = ug.User.nickname ,role = ug.role.name});
            }

            page.members = members;

            return Ok(page);
        }
    }
}
