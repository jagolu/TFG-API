using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.Alive.Controllers
{
    [Route("Alive/[action]")]
    [ApiController]
    public class WatchNotificationController : ControllerBase
    {
        private ApplicationDBContext _context;

        public WatchNotificationController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("WatchNotification")]
        public IActionResult LoginNotifications(string id)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!user.open) return BadRequest(new { error = "YoureBanned" });

            try
            {
                _context.Entry(user).Collection("notifications").Load();
                List<Notifications> nots = user.notifications.Where(n => n.id.ToString() == id).ToList();
                if(nots.Count() != 1)
                {
                    return Ok();
                }
                try
                {
                    _context.Remove(nots.First());
                    _context.SaveChanges();

                    return Ok();
                }
                catch (Exception)
                {
                    return StatusCode(500);
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
