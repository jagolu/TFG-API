using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Areas.Alive.Util;
using API.Areas.DirectMessages.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace API.Areas.DirectMessages.Controllers
{
    [Route("DirectMessages/[action]")]
    [ApiController]
    public class SendDMMessageController : ControllerBase
    {
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The database context of the application</value>
        private ApplicationDBContext _context;

        /// <value>The scope factory to get the updated database context</value>
        private readonly IServiceScopeFactory _scopeFactory;

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
        /// <param name="sf">The service scope factory</param>
        /// <param name="hub">The notification hub</param>
        public SendDMMessageController(ApplicationDBContext context, IServiceScopeFactory sf, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _scopeFactory = sf;
            _hub = hub;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //
        
        [HttpPost]
        [Authorize]
        [ActionName("SendDMMessage")]
        /// <summary>
        /// Sends a message to a dm conversation
        /// </summary>
        /// <param name="order">The info to send the message</param>
        /// See <see cref="Areas.DirectMessages.Models.SendDMMessage"/> to know the param structure
        /// <returns>IActionResult of the send message action</returns>
        /// See <see cref="Areas.DirectMessages.Models.DMRoom"/> to know the response structure
        public async Task<IActionResult> sendMsg([FromBody] SendDMMessage order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            DirectMessageTitle title = new DirectMessageTitle();
            if (!user.open) return BadRequest(new { error = "YoureBanned" });

            if(!checkOrder(ref title, order.dmId, user))
            {
                return BadRequest();
            }

            try
            {
                bool isAdmin = AdminPolicy.isAdmin(user, _context);
                DirectMessageMessages msg = new DirectMessageMessages
                {
                    message = order.message,
                    isAdmin = isAdmin,
                    DirectMessageTitle = title
                };

                addUnreadMessages(title, isAdmin);
                _context.Add(msg);
                _context.SaveChanges();
                await sendMailAndSendNotification(title, user);

                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                    DirectMessageTitle dbTitle = dbContext.DirectMessagesTitle.Where(t => t.id == title.id).First();
                    User dbuser = dbContext.User.Where(u => u.id == user.id).First();

                    DMRoom room = new DMRoom(dbTitle, dbuser, dbContext);
                    return Ok(room);
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Check the param request
        /// </summary>
        /// <param name="title">A new DirectMessageTitle object to save the dm conversation on it</param>
        /// <param name="dmId">The id of the dm conversation</param>
        /// <param name="user">The caller of the function</param>
        /// <returns>True if the params request are good, false otherwise</returns>
        private bool checkOrder(ref DirectMessageTitle title, string dmId, User user)
        {
            _context.Entry(user).Collection("directMessages").Load();
            List<DirectMessageTitle> dms = user.directMessages.Where(dm => dm.id.ToString() == dmId).ToList();
            dms.AddRange(_context.DirectMessagesTitle.Where(dm => dm.id.ToString() == dmId && dm.receiver == user).ToList());
            if (dms.Count() != 1)
            {
                return false;
            }

            title = dms.First();
            if (title.closed)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Add an unread message to the receiver of the message
        /// </summary>
        /// <param name="title">The dm conversation</param>
        /// <param name="isAdmin">True if the sender is an admin, false otherwise</param>
        private void addUnreadMessages(DirectMessageTitle title, bool isAdmin)
        {
            if (isAdmin)
            {
                title.unreadMessagesForUser++;
            }
            else
            {
                title.unreadMessagesForAdmin++;
            }
            title.lastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Send a email and notification to the receiver
        /// </summary>
        /// <param name="title">The dm conversation</param>
        /// <param name="caller">The caller of the function</param>
        private async Task sendMailAndSendNotification(DirectMessageTitle title, User caller)
        {
            //If the recv has more than 1 unread messages doesn't need another email
            _context.Entry(caller).Reference("role").Load();
            bool callerIsAdmin = caller.role == RoleManager.getAdmin(_context);
            if (title.unreadMessagesForUser > 1 && callerIsAdmin) return;
            if (title.unreadMessagesForAdmin> 1 && !callerIsAdmin) return;

            _context.Entry(title).Reference("Sender").Load();
            _context.Entry(title).Reference("receiver").Load();
            User theUser = title.Sender.id == caller.id ? title.Sender : title.receiver;
            User notificationReceiver = title.Sender.id == caller.id ? title.receiver : title.Sender;

            if (callerIsAdmin) EmailSender.sendDMNotification(theUser.email, theUser.nickname, title.title);
            await sendNotification(notificationReceiver);
        }
        
        /// <summary>
        /// Send the notification to the user
        /// </summary>
        /// <param name="recv">The receiver of the notification</param>
        private async Task sendNotification(User recv)
        {
            await SendNotification.send(_hub, "", recv, Alive.Models.NotificationType.RECEIVED_DM, _context);
        }
    }
}
