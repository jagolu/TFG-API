using System;
using System.Collections.Generic;
using API.Areas.Home.Models;
using API.Areas.Home.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Home.Controllers
{
    [Route("Home/[action]")]
    [ApiController]
    public class AuthorizedNewsController : ControllerBase
    {
        private ApplicationDBContext _context;

        public AuthorizedNewsController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("AuthHome")]
        public IActionResult getAuth()
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");

            try
            {
                List<NewMessage> retMessage = GetNews.getAuthNews(user.id, _context);

                return Ok(retMessage);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}