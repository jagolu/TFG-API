using System.Collections.Generic;
using System.Linq;
using API.Areas.Admin.Models;
using API.Areas.Admin.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Admin.Controllers
{
    [Route("Admin/[action]")]
    [ApiController]
    public class SearchAllUserController : ControllerBase
    {
        //
        // ────────────────────────────────────────────────────────────  ──────────
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
        public SearchAllUserController(ApplicationDBContext context)
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
        [ActionName("GetAllUsers")]
        /// <summary>
        /// Get all the users from the database who are not admin users
        /// </summary>
        /// <returns>A list with all the not-admin users</returns>
        /// See <see cref="Areas.Admin.Models.UserSearchInfo"/> to know the response structure
        public List<UserSearchInfo> getAllUsers()
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!AdminPolicy.isAdmin(user, _context)) return new List<UserSearchInfo>();
            List<UserSearchInfo> userRet = new List<UserSearchInfo>();
            Role adminRole = RoleManager.getAdmin(_context);

            return MakeListUserSearchInfo.make(_context.User.Where(u => u.role != adminRole).Take(25).ToList(), _context);
        }
    }
}
