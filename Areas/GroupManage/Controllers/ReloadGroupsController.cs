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
        public ReloadGroupsController(ApplicationDBContext context)
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
        [ActionName("ReloadUserGroups")]
        /// <summary>
        /// Gives an updated list of the groups of the user
        /// </summary>
        /// <returns>A list of the groups of the user</returns>
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
