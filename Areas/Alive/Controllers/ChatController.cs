using API.Areas.Alive.Models;
using API.Areas.Alive.Util;
using API.Areas.GroupManage.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace API.Areas.Alive.Controllers
{
    [Route("Alive/[action]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private IHubContext<ChatHub> _hub;
        private ApplicationDBContext _context;

        public ChatController(ApplicationDBContext context, IHubContext<ChatHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        [ActionName("ChatLogin")]
        [Authorize]
        public async System.Threading.Tasks.Task<IActionResult> LoginChatAsync([Required]string groupName)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            Group group = new Group();
            UserGroup ugCaller = new UserGroup();

            if(!UserInGroup.checkUserInGroup(user.id, ref group, groupName, ref ugCaller, _context))
            {
                return BadRequest();
            }

            try
            {
                _context.Entry(group).Collection("chatMessages").Load();
                ChatLogin retMessages = new ChatLogin();
                retMessages.callerPublicId = user.publicId;
                retMessages.messages = new List<ChatMessage>();
                group.chatMessages.ToList().ForEach(msg =>
                {
                    retMessages.messages.Add(new ChatMessage(msg, _context));
                });

                await sendWelcomeMessageAsync(groupName, user);

                return Ok(retMessages);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        public async System.Threading.Tasks.Task sendWelcomeMessageAsync(string groupName, User user)
        {
            await _hub.Clients.All.SendAsync(groupName,
                new ChatMessage
                {
                    group = "",
                    username = "",
                    publicUserId = user.publicId,
                    role = "",
                    message = user.nickname + " is online",
                    time = DateTime.Now
                }
            );
        }
    }
}
