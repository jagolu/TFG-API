using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.Admin.Models;
using API.Areas.Alive.Models;
using API.Areas.Alive.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace API.Areas.Admin.Controllers
{
    [Route("Admin/[action]")]
    [ApiController]
    public class BanGroupController : ControllerBase
    {
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //
        
        /// <value>The database context of the application</value>
        private ApplicationDBContext _context;
        
        /// <value>The notification hub</value>
        private IHubContext<NotificationHub> _hub;

        
        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //
        
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="hub">The notification hub</param>
        public BanGroupController(ApplicationDBContext context, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //
        
        [HttpPost]
        [Authorize]
        [ActionName("BanGroup")]
        /// <summary>
        /// Bans a group
        /// </summary>
        /// <param name="order">The info</param>
        /// See <see cref="Areas.Admin.Models.BanGroup"/> to see the param info
        /// <returns>The IActionResult of the ban action</returns>
        public IActionResult banGroup([FromBody] BanGroup order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            Group targetGroup = new Group();
            if (!AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");

            if (!existGroup(ref targetGroup, order.groupName))
            {
                return BadRequest();
            }
            if (validOrder(targetGroup, order.ban))
            {
                return BadRequest();
            }

            try
            {
                targetGroup.open = !targetGroup.open;
                _context.Update(targetGroup);
                _context.SaveChanges();

                sendNews(targetGroup, order.ban);

                string retMessage = order.ban ? "SuccessfullGroupBan" : "SuccessfullGroupUnban";
                return Ok(new { success = retMessage });
            }
            catch (Exception)
            {
                return BadRequest(500);
            }
        }


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //
        
        /// <summary>
        /// Check if a group exists
        /// </summary>
        /// <param name="group">A new group object, to save the group on it</param>
        /// <param name="name">The name of the group</param>
        /// <returns>True if the group exists, false otherwise</returns>
        private bool existGroup(ref Group group, string name)
        {
            List<Group> existGroup = _context.Group.Where(u => u.name == name).ToList();
            if (existGroup.Count() != 1)
            {
                return false;
            }

            group = existGroup.First();

            return true;
        }

        /// <summary>
        /// Checks if the http order is correct or not
        /// </summary>
        /// <param name="group">The group which we want to do the operation</param>
        /// <param name="order">The operation to do. True to ban the group, false to unban it</param>
        /// <returns>True if the operation can be done, false otherwise</returns>
        private bool validOrder(Group group, bool order)
        {
            bool userBlock = group.open;

            return userBlock != order;
        }

        /// <summary>
        /// Send the news and notifications to the group members
        /// </summary>
        /// <param name="group">The group which has been banned or unbanned</param>
        /// <param name="ban">True if the group has been banned, false otherwise</param>
        private void sendNews(Group group, bool ban)
        {
            _context.Entry(group).Collection("users").Load();

            group.users.ToList().ForEach(async u =>
            {
                _context.Entry(u).Reference("User").Load();
                User recv = u.User;
                Home.Util.GroupNew.launch(recv, group, null, Home.Models.TypeGroupNew.BAN_GROUP, ban, _context);

                NotificationType type = ban ? NotificationType.BAN_GROUP : NotificationType.UNBAN_GROUP;
                await SendNotification.send(_hub, group.name, recv, type, _context);
            });
        }
    }
}
