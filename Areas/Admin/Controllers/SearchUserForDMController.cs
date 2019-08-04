using System.Collections.Generic;
using System.Linq;
using API.Areas.Admin.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Admin.Controllers
{
    [Route("Admin/[action]")]
    [ApiController]
    public class SearchUseForDM : ControllerBase
    {
        private ApplicationDBContext _context;

        public SearchUseForDM(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("SearchForDM")]
        public List<SearchUserDM> search(string findTo)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!AdminPolicy.isAdmin(user, _context)) return new List<SearchUserDM>();

            //The request name is empty
            if (findTo == null || findTo.Length == 0)
            {
                return new List<SearchUserDM>();
            }

            Role adminRole = RoleManager.getAdmin(_context);
            List<User> userWithSameMail = _context.User.Where(u =>
                (u.email.ToLower().Contains(findTo.ToLower().Trim()) ||
                u.nickname.ToLower().Contains(findTo.ToLower().Trim())) &&
                u.role != adminRole
            ).ToList();

            List<SearchUserDM> usersRet = new List<SearchUserDM>();
            userWithSameMail.ForEach(u =>usersRet.Add(new SearchUserDM(u)));

            return usersRet;
        }
    }
}
