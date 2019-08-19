using System.Collections.Generic;
using System.Linq;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Areas.GroupManage.Controllers
{
    [Route("Group/[action]")]
    [ApiController]
    public class ReloadGroupsController : ControllerBase
    {
        private ApplicationDBContext _context;

        public ReloadGroupsController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("ReloadUserGroups")]
        public List<string> reloadUserGroups()
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!user.open) return new List<string>();
            List<string> groupsInfo = new List<string>();
            _context.Entry(user).Collection("groups").Load();

            user.groups.Where(g => !g.blocked).ToList().ForEach(group =>
            {
                _context.Entry(group).Reference("Group").Load();

                groupsInfo.Add(group.Group.name);
            });

            return groupsInfo;
        }
    }
}
