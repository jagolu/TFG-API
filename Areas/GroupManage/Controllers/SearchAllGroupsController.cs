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
        public SearchAllGroupsController(ApplicationDBContext context)
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
        [ActionName("GetAllGroups")]
        /// <summary>
        /// Get all the groups that the user is not join in
        /// </summary>
        /// <returns>A list with the groups that the user is not join in</returns>
        /// See <see cref="Areas.GroupManage.Models.GroupInfo"/> to see the response structure
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
