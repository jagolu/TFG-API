using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.UserInfo.Models;
using API.Data;
using API.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.UserInfo.Controllers
{
    [Route("User/[action]")]
    [ApiController]
    public class GetUserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public GetUserController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("UserInfo")]
        public IActionResult getUser()
        {
            try {
                var authToken = HttpContext.Request?.Headers["Authorization"];
                string email = TokenGenerator.getEmailClaim(TokenGenerator.getBearerToken(authToken.Value));

                User user = _context.User.Where(u => u.email == email).First();

                _context.Entry(user).Reference("role").Load();

                UserData userShow = new UserData {
                    email = user.email,
                    nickname = user.nickname,
                    img = user.profileImg,
                    user_role = user.role.name,
                    rolesGroup = getRoleGroups(user),
                    timeSignUp = user.dateSignUp,
                    password = user.password != null
                };

                return Ok(userShow);    

            } catch (Exception) {
                return StatusCode(500);
            }
        }

        private List<RoleGroup> getRoleGroups(User u)
        {
            List<RoleGroup> roleGroups = new List<RoleGroup>();

            u.groups.ToList().ForEach(
                i => roleGroups.Add(new RoleGroup {
                    group = i.Group.name,
                    groupType = i.Group.type ? "OFICIAL" : "VIRTUAL",
                    role = i.role.name
                })
            );

            return roleGroups;
        }
    }
}
