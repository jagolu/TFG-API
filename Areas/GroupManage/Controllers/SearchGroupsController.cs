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
    public class SearchGroupsController : ControllerBase
    {
        private ApplicationDBContext _context;

        public SearchGroupsController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("SearchGroup")]
        public List<GroupInfo> searchGroupByName(string name)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            name = name.ToLower().Trim();

            if (!user.open) return new List<GroupInfo>();

            //The request name is empty
            if (name == null || name.Length == 0)
            {
                return new List<GroupInfo>();
            }

            bool isAdmin = AdminPolicy.isAdmin(user, _context);
            List<Group> groupsWithTheSameName = !isAdmin ?
                _context.Group.Where(g => g.name.ToLower().Contains(name) && g.open).ToList() :
                _context.Group.Where(g => g.name.ToLower().Contains(name)).ToList();

            return MakeGroupInfoList.make(groupsWithTheSameName, AdminPolicy.isAdmin(user, _context), _context);
        }
    }
}
