using System.Linq;
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
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
            var groups = _context.Group.Where(g => g.name == groupName);

            // If the group doesn't exist
            if (groups.Count() != 1)
            {
                return BadRequest();
            }

            Group group = groups.First();
            _context.Entry(group).Collection("users").Load();

            // If the user doesn't belong to the group
            if (group.users.Where(u => u.userId == user.id && !u.blocked).Count() != 1)
            {
                return BadRequest();
            }

            return Ok(GroupPageManager.GetPage(user, group, _context));
        }
    }
}
