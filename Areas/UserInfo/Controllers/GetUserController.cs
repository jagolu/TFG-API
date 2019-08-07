using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.UserInfo.Models;
using API.Areas.UserInfo.Util;
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
                if (!user.open) return BadRequest(new { error = "YoureBanned" });

                _context.Entry(user).Reference("role").Load();

                UserData userShow = new UserData {
                    email = user.email,
                    nickname = user.nickname,
                    img = user.profileImg,
                    groups = GroupsOfUser.get(user, _context),
                    timeSignUp = user.dateSignUp
                };

                return Ok(userShow);    

            } catch (Exception) {
                return StatusCode(500);
            }
        }
    }
}
