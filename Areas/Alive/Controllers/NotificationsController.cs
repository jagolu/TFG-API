using API.Areas.Alive.Models;
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
    public class NotificationsController : ControllerBase
    {
        private ApplicationDBContext _context;

        public NotificationsController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("NotificationsLogin")]
        public IActionResult LoginNotifications()
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!user.open) return BadRequest(new { error = "YoureBanned" });

            try
            {
                _context.Entry(user).Collection("notifications").Load();
                List<NotificationMessage> notifications = new List<NotificationMessage>();

                user.notifications.OrderByDescending(n => n.time).ToList().ForEach(n =>
                {
                    notifications.Add(new NotificationMessage { id = n.id.ToString(), message = n.message });
                });

                return Ok(new LoginNotifications { publicUserid= user.publicid.ToString(), messages=notifications});
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
