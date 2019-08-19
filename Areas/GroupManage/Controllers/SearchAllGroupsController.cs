using System.Collections.Generic;
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
    public class SearchAllGroupsController : ControllerBase
    {
        private ApplicationDBContext _context;

        public SearchAllGroupsController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("GetAllGroups")]
        public List<GroupInfo> getAllGroups()
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!user.open) return new List<GroupInfo>();
            bool isAdmin = AdminPolicy.isAdmin(user, _context);
            List<Group> groups = !isAdmin ? _context.Group.Where(g => g.open).Take(25).ToList()
                                          : _context.Group.Take(25).ToList();

            return MakeGroupInfoList.make(groups, AdminPolicy.isAdmin(user, _context), _context);
        }
    }
}
