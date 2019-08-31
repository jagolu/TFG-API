using System;
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
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The database context of the application</value>
        private readonly ApplicationDBContext _context;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The database context</param>
        public GetUserController(ApplicationDBContext context)
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
        [ActionName("UserInfo")]
        /// <summary>
        /// Get the user profile
        /// </summary>
        /// <returns>The IActionResult of the get user action</returns>
        /// See <see cref="Areas.UserInfo.Models.UserData"/> to know the response structure
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
