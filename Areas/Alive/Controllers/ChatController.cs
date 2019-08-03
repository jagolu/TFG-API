using API.Areas.Alive.Models;
using API.Areas.GroupManage.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static API.Areas.Alive.Models.ChatLogin;
using static API.Areas.Alive.Models.ChatLogin.ChatUserMesssages;

namespace API.Areas.Alive.Controllers
{
    [Route("Alive/[action]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private ApplicationDBContext _context;

        public ChatController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("ChatLogin")]
        public IActionResult LoginChat([Required]string groupName)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if(!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
            Group group = new Group();
            UserGroup ugCaller = new UserGroup();

            if(!UserFromGroup.isOnIt(user.id, ref group, groupName, ref ugCaller, _context))
            {
                return BadRequest();
            }

            try
            {
                _context.Entry(group).Collection("chatMessages").Load();
                ChatLogin retMessages = new ChatLogin();
                retMessages.callerPublicId = user.publicId;
                retMessages.group = group.name;
                retMessages.userMessages= filterMessages(group.chatMessages.OrderBy(m => m.time).ToList());

                return Ok(retMessages);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        public List<ChatUserMesssages> filterMessages(List<GroupChatMessage> msgs)
        {
            string lastUserId = "";
            List<ChatUserMesssages> messages = new List<ChatUserMesssages>();

            msgs.ForEach(msg =>
            {
                if (msg.publicUserId != lastUserId)
                {
                    _context.Entry(msg).Reference("role").Load();
                    lastUserId = msg.publicUserId;
                    List<SingleUserChatMessage> newMessages = new List<SingleUserChatMessage>();
                    newMessages.Add(new SingleUserChatMessage { message = msg.message, time = msg.time });
                    messages.Add(new ChatUserMesssages
                    {
                        username = msg.username,
                        publicUserId = msg.publicUserId,
                        role = msg.role.name,
                        messages = newMessages
                    });
                }
                else
                {
                    messages.Last().messages.Add(new SingleUserChatMessage { message = msg.message, time = msg.time });
                }
            });

            return messages;
        }
    }
}
