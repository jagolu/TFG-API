using System;
using API.Areas.DirectMessages.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.DirectMessages.Controllers
{
    [Route("DirectMessages/[action]")]
    [ApiController]
    public class LoadTitleDMController : ControllerBase
    {
        private ApplicationDBContext _context;

        public LoadTitleDMController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("LoadDMTitles")]
        public IActionResult loadTitles()
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!user.open) return BadRequest(new { error = "YoureBanned" });

            try
            {
                return Ok(LoadTitles.load(user, _context));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
