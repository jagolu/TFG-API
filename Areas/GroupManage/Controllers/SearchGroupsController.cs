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
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The database context of the application</value>
        private ApplicationDBContext _context;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The database context</param>
        public SearchGroupsController(ApplicationDBContext context)
        {
            _context = context;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        [HttpGet]
        [Authorize]
        [ActionName("SearchGroup")]
        /// <summary>
        /// Get a list of groups with a similar name
        /// </summary>
        /// <param name="name">The name of the group to find</param>
        /// <returns>A list of group with a similar name</returns>
        /// See <see cref="Areas.GroupManage.Models.GroupInfo"/> to see the response structure
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
