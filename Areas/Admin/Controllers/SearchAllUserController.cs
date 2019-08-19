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
        private ApplicationDBContext _context;

        public SearchAllUserController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("GetAllUsers")]
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
