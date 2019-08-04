using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace API.Areas.Alive.Controllers
{
    [Route("Alive/[action]")]
    [ApiController]
    public class WatchAllNotificationsController : ControllerBase
    {
        private ApplicationDBContext _context;

        public WatchAllNotificationsController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("WatchAllNotifications")]
        public IActionResult ReadAllNotifications()
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!user.open) return BadRequest(new { error = "YoureBanned" });

            try
            {
                _context.Entry(user).Collection("notifications").Load();
                _context.RemoveRange(user.notifications.ToList());
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
