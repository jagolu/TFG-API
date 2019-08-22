﻿using System;
using System.Linq;
using API.Areas.Alive.Models;
using API.Areas.Alive.Util;
using API.Areas.GroupManage.Models;
using API.Areas.GroupManage.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace API.Areas.GroupManage.Controllers
{
    [Route("Group/[action]")]
    [ApiController]
    public class RemoveUserFromGroupController : ControllerBase
    {
        private ApplicationDBContext _context;
        private readonly IServiceScopeFactory _scopeFactory;
        private IHubContext<NotificationHub> _notificationHub;
        private IHubContext<ChatHub> _chatHub;

        public RemoveUserFromGroupController(ApplicationDBContext context, IServiceScopeFactory sf, IHubContext<NotificationHub> notificationHub, IHubContext<ChatHub> chatHub)
        {
            _context = context;
            _scopeFactory = sf;
            _notificationHub = notificationHub;
            _chatHub = chatHub;
        }

        [HttpPost]
        [Authorize]
        [ActionName("RemoveUser")]
        public async System.Threading.Tasks.Task<IActionResult> removeUserAsync([FromBody] KickUser order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to kick the user from the group
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
            UserGroup targetUser = new UserGroup();
            Group group = new Group();

            if (!GroupAdminFuncionlities.checkFuncionality(user, ref group, order.groupName, ref targetUser, order.publicId, _context, GroupAdminFuncionality.REMOVE_USER, false))
            {
                return BadRequest();
            }
            if (!group.open) return BadRequest(new { error = "GroupBanned" });

            try
            {
                _context.Entry(targetUser).Reference("User").Load();
                User sendNew = targetUser.User;
                Guid targetUserid = targetUser.User.id; 
                await QuitUserFromGroup.quitUser(targetUser, _context, _notificationHub);
                await KickChatNotification.sendKickMessageAsync(group.name, targetUser.User.publicid, _chatHub);
                InteractionManager.manageInteraction(targetUser.User, group, interactionType.KICKED, _context);

                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                    group = dbContext.Group.Where(g => g.name == order.groupName).First();
                    User recv = dbContext.User.Where(u => u.id == targetUserid).First();
                    user = dbContext.User.Where(u => u.id == user.id).First();

                    Home.Util.GroupNew.launch(sendNew, group, null, Home.Models.TypeGroupNew.KICK_USER_USER, false, dbContext);
                    Home.Util.GroupNew.launch(sendNew, group, null, Home.Models.TypeGroupNew.KICK_USER_GROUP, false, dbContext);
                    await SendNotification.send(_notificationHub, group.name, recv, NotificationType.KICKED_GROUP, dbContext);

                    return Ok(GroupPageManager.GetPage(user, group, dbContext));
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
