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
    public class SearchUserController : ControllerBase
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
        /// <param name="context">The context of the database</param>
        public SearchUserController(ApplicationDBContext context)
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
        [ActionName("SearchUser")]
        /// <summary>
        /// Search a specific user
        /// </summary>
        /// <param name="toFind">The username or email of the user to find</param>
        /// <returns>A list with all the not-admin users who username or email match with the param</returns>
        /// See <see cref="Areas.Admin.Models.UserSearchInfo"/> to know the response structure
        public List<UserSearchInfo> searchUser(string toFind)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!AdminPolicy.isAdmin(user, _context)) return new List<UserSearchInfo>();

            //The request name is empty
            if (toFind == null || toFind.Length == 0)
            {
                return new List<UserSearchInfo>();
            }

            Role adminRole = RoleManager.getAdmin(_context);
            List<User> userWithSameMail = _context.User.Where(u =>
                (u.email.ToLower().Contains(toFind.ToLower().Trim()) ||
                u.nickname.ToLower().Contains(toFind.ToLower().Trim())) &&
                u.role != adminRole
            ).ToList();

            return MakeListUserSearchInfo.make(userWithSameMail, _context);
        }
    }
}
