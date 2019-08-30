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
using static API.Areas.Alive.Models.ChatLogin.ChatUserMessages;

namespace API.Areas.Alive.Controllers
{
    [Route("Alive/[action]")]
    [ApiController]
    public class ChatController : ControllerBase
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
        public ChatController(ApplicationDBContext context)
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
        [ActionName("ChatLogin")]
        /// <summary>
        /// Log the user in a group chat
        /// </summary>
        /// <param name="groupName">The name of the group</param>
        /// <returns>IActionResult of the log chat action</returns>
        /// See <see cref="Areas.Alive.Models.ChatLogin"/> to see the response structure
        public IActionResult loginChat([Required]string groupName)
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
                retMessages.callerPublicId = user.publicid;
                retMessages.group = group.name;
                retMessages.userMessages= filterMessages(group.chatMessages.OrderBy(m => m.time).ToList());

                return Ok(retMessages);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }


        //
        // ──────────────────────────────────────────────────────────────────────────  ──────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Filter the messages on the response structure
        /// </summary>
        /// <param name="msgs">The messages of a group chat</param>
        /// <returns>The group chat messages on <see cref="Areas.Alive.Models.ChatLogin"/></returns>
        private List<ChatUserMessages> filterMessages(List<GroupChatMessage> msgs)
        {
            string lastUserId = "";
            List<ChatUserMessages> messages = new List<ChatUserMessages>();

            msgs.ForEach(msg =>
            {
                if (msg.publicUserid != lastUserId)
                {
                    _context.Entry(msg).Reference("role").Load();
                    lastUserId = msg.publicUserid;
                    List<SingleUserChatMessage> newMessages = new List<SingleUserChatMessage>();
                    newMessages.Add(new SingleUserChatMessage { message = msg.message, time = msg.time });
                    messages.Add(new ChatUserMessages
                    {
                        username = msg.username,
                        publicUserId = msg.publicUserid,
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
