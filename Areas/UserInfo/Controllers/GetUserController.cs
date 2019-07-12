using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.UserInfo.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
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
                User user = TokenUserManager.getUserFromToken(HttpContext, _context);

                _context.Entry(user).Reference("role").Load();

                UserData userShow = new UserData {
                    email = user.email,
                    nickname = user.nickname,
                    img = user.profileImg,
                    user_role = user.role.name,
                    rolesGroup = getRoleGroups(user),
                    timeSignUp = user.dateSignUp
                };

                return Ok(userShow);    

            } catch (Exception) {
                return StatusCode(500);
            }
        }

        private List<RoleGroup> getRoleGroups(User user)
        {
            List<RoleGroup> roleGroups = new List<RoleGroup>();
            _context.Entry(user).Collection("groups").Load();

            user.groups.ToList().ForEach(
                group => {
                    _context.Entry(group).Reference("Group").Load();
                    _context.Entry(group).Reference("role").Load();

                    roleGroups.Add(new RoleGroup
                    {
                        name = group.Group.name,
                        type = group.Group.type,
                        role = group.role.name
                    });
                }
            );

            return roleGroups;
        }
    }
}
