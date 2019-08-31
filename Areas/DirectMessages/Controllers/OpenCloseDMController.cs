using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Areas.Alive.Models;
using API.Areas.Alive.Util;
using API.Areas.DirectMessages.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace API.Areas.DirectMessages.Controllers
{
    [Route("DirectMessages/[action]")]
    [ApiController]
    public class OpenCloseDMController : ControllerBase
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
        public OpenCloseDMController(ApplicationDBContext context, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //
        
        [HttpGet]
        [Authorize]
        [ActionName("CloseDM")]
        /// <summary>
        /// Closes or open a dm conversation
        /// </summary>
        /// <param name="id">The id of the dm conversation</param>
        /// <param name="openOrder">"1" to open de conversation, "0" to clse it</param>
        /// <returns>The IActionResult of the open/close action</returns>
        /// See <see cref="Areas.DirectMessages.Models.DMRoom"/> to know the response structure
        public async Task<IActionResult> close(string id, string openOrder)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            DirectMessageTitle title = new DirectMessageTitle();
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (!AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
            if (openOrder != "1" && openOrder != "0") return BadRequest();
            bool open = openOrder == "1";

            if(!checkOrder(ref title, id, open))
            {
                return BadRequest();
            }
            if (!checkAdminInDM(title, user))
            {
                return BadRequest();
            }
            try
            {
                sendClosedMessage(title, open);
                title.closed = !open;
                _context.SaveChanges();

                await sendMailAndNotification(title, user, open);
                DMRoom room = new DMRoom(title, user, _context);

                return Ok(room);
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
        /// Checks the open/close request
        /// </summary>
        /// <param name="title">A new DirectMessageTitle object, to save the dm conversation object</param>
        /// <param name="id">The id of the dm conversation</param>
        /// <param name="openClose">True to open the dm, false to close it</param>
        /// <returns></returns>
        private bool checkOrder(ref DirectMessageTitle title, string id, bool openClose)
        {
            List<DirectMessageTitle> titles = _context.DirectMessagesTitle.Where(dm => dm.id.ToString() == id).ToList();

            if(titles.Count() != 1)
            {
                return false;
            }

            title = titles.First();
            if (title.closed != openClose)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the admin is on the dm conversation
        /// </summary>
        /// <param name="title">The dm conversation</param>
        /// <param name="admin">The admim of the conversation</param>
        /// <returns></returns>
        private bool checkAdminInDM(DirectMessageTitle title, User admin)
        {
            _context.Entry(title).Reference("receiver").Load();

            return title.senderid == admin.id || title.receiver.id == admin.id;
        }

        /// <summary>
        /// Send the open/close message to the dm conversation
        /// </summary>
        /// <param name="title"></param>
        /// <param name="open"></param>
        private void sendClosedMessage(DirectMessageTitle title, bool open)
        {
            DirectMessageMessages msg = new DirectMessageMessages
            {
                message = open ? "El administrador encargado de la convesación la ha reabierto" : "Esta conversación ha sido cerrada",
                isAdmin = true,
                DirectMessageTitle = title
            };
            _context.Add(msg);
            title.unreadMessagesForAdmin = 0;
            title.unreadMessagesForUser++;
            title.lastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Send a email and a notification to the user
        /// </summary>
        /// <param name="title">The dm conversation</param>
        /// <param name="caller">The user who has open/close the conversation</param>
        /// <param name="open">True if the conversation has just open, false otherwise</param>
        /// <returns></returns>
        private async Task sendMailAndNotification(DirectMessageTitle title, User caller, bool open)
        {
            _context.Entry(title).Reference("Sender").Load();
            _context.Entry(title).Reference("receiver").Load();
            User theUser = title.Sender.id == caller.id ? title.Sender : title.receiver;
            User notificationReceiver = title.Sender.id == caller.id ? title.receiver : title.Sender;

            EmailSender.sendOpenCloseDMNotification(theUser.email, theUser.nickname, title.title, open);

            await sendNotification(notificationReceiver, open);
        }
        
        /// <summary>
        /// Send the notification to the user
        /// </summary>
        /// <param name="recv">The receiver of the notification</param>
        /// <param name="open">True if the conversation has been opened, false otherwise</param>
        private async Task sendNotification(User recv, bool open)
        {

            NotificationType type = open ? NotificationType.REOPEN_DM : NotificationType.CLOSE_DM;

            await SendNotification.send(_hub, "", recv, type, _context);
        }
    }
}
