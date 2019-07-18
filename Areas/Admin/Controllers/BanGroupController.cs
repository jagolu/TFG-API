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
        private ApplicationDBContext _context;
        private IHubContext<NotificationHub> _hub;

        public BanGroupController(ApplicationDBContext context, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }


        [HttpPost]
        [Authorize]
        [ActionName("BanGroup")]
        public IActionResult banUser([FromBody] BanGroup order)
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

        private bool validOrder(Group group, bool order)
        {
            bool userBlock = group.open;

            return userBlock != order;
        }

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
