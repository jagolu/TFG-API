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
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The database context of the application</value>
        private ApplicationDBContext _context;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The context of the database</param>
        public NotificationsController(ApplicationDBContext context)
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
        [ActionName("NotificationsLogin")]
        /// <summary>
        /// Log a user in to the notifications
        /// </summary>
        /// <returns>IActionResult of the log notifications action</returns>
        /// See <see cref="Areas.Alive.Models.LoginNotifications"/> to see the response structure
        public IActionResult loginNotifications()
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
