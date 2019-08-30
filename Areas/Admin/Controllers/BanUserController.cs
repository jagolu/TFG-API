using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.Admin.Models;
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
    public class BanUserController : ControllerBase
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
        public BanUserController(ApplicationDBContext context, IHubContext<NotificationHub> hub)
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
        [ActionName("BanUser")]
        /// <summary>
        /// Bans a user
        /// </summary>
        /// <param name="order">The info</param>
        /// See <see cref="Areas.Admin.Models.BanUser"/> to see the param info
        /// <returns>The IActionResult of the ban action</returns>
        public IActionResult banUser([FromBody] BanUser order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            User targetUser = new User();
            if (!AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
                        
            if(!existUser(ref targetUser, order.publicUserId))
            {
                return BadRequest();
            }
            if(validOrder(targetUser, order.ban))
            {
                return BadRequest();
            }

            try
            {
                targetUser.open = !targetUser.open;
                _context.Update(targetUser);

                _context.SaveChanges();
                if(order.ban) manageGroups(targetUser);

                EmailSender.sendBanNotification(targetUser.email, targetUser.nickname, order.ban);

                string retMessage = order.ban ? "SuccessfullUserBan" : "SuccessfullUserUnban";
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
        /// Checks if a user exists
        /// </summary>
        /// <param name="user">A new User object (ref param)</param>
        /// <param name="publicUserId">The public id of the user</param>
        /// <returns>True if the user exists, false otherwise</returns>
        private bool existUser(ref User user, string publicUserId)
        {
            List<User> existUser = _context.User.Where(u => u.publicid == publicUserId).ToList();
            if (existUser.Count() != 1)
            {
                return false;
            }

            user = existUser.First();

            return true;
        }

        /// <summary>
        /// Checks if the http order is correct or not
        /// </summary>
        /// <param name="user">The user which we want to do the operation</param>
        /// <param name="order">The operation to do. True to ban the user, false to unban it</param>
        /// <returns>True if the operation can be done, false otherwise</returns>
        private bool validOrder(User user, bool order)
        {
            _context.Entry(user).Reference("role").Load();
            Role admin = RoleManager.getAdmin(_context);

            if(user.role == admin)
            {
                return false;
            }

            return user.open != order;
        }

        /// <summary>
        /// Manage the groups which the user belongs to
        /// </summary>
        /// <param name="user">The user who has been banned or unbanned</param>
        private void manageGroups(User user)
        {
            _context.Entry(user).Collection("groups").Load();
            Role maker = RoleManager.getGroupMaker(_context);
            Role admin = RoleManager.getGroupAdmin(_context);
            Role normal = RoleManager.getGroupNormal(_context);
            user.groups.ToList().ForEach(async g =>
            {
                List<UserGroup> members = GroupManage.Util.QuitUserFromGroup.getValidUsersInGroup(g, _context);
                _context.Entry(g).Reference("role").Load();

                if (members.Count() > 0)
                {
                    if(g.role == maker)
                    {
                        await GroupManage.Util.QuitUserFromGroup.manageQuitMaker(members, maker, admin, normal, false, _context, _hub);
                    }

                    g.role = normal;
                    _context.Update(g);
                    _context.SaveChanges();
                }
                else
                {
                    _context.Entry(g).Reference("Group").Load();
                    GroupManage.Util.RemoveGroup.remove(g.Group, _context, _hub);
                }
            });
        }
    }
}
