using System;
using System.Collections.Generic;
using API.Areas.Admin.Models;
using API.Areas.Home.Models;
using API.Areas.Home.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Admin.Controllers
{
    [Route("Admin/[action]")]
    [ApiController]
    public class LaunchNewController : ControllerBase
    {
        private ApplicationDBContext _context;

        public LaunchNewController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("LaunchNew")]
        public IActionResult launchNew([FromBody] Message message)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");

            try
            {
                _context.Add(new New
                {
                    title = "Aviso de los administradores!",
                    message = message.message
                });

                _context.SaveChanges();
                List<NewMessage> retMessage = GetNews.getStandNews(_context);

                return Ok(retMessage);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
