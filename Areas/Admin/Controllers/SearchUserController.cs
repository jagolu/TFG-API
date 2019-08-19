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
        private ApplicationDBContext _context;

        public SearchUserController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("SearchUser")]
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
