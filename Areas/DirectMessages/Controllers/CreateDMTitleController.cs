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
    public class CreateDMTitleController : ControllerBase
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
        public CreateDMTitleController(ApplicationDBContext context, IHubContext<NotificationHub> hub)
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
        [ActionName("CreateDMTitle")]
        /// <summary>
        /// Create a direct conversation
        /// </summary>
        /// <param name="order">The info to create the DM</param>
        /// See <see cref="Areas.DirectMessages.Models.CreateDMTitle"/> to know the param structure
        /// <returns>IActionResult of the createTitle structure with the id of the dm created</returns>
        public async Task<IActionResult> createTitle([FromBody] CreateDMTitle order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            User receiver = new User();
            if (!user.open) return BadRequest(new { error = "YoureBanned" });

            if(!checkSenderAndReceiver(user, order.emailReceiver))
            {
                return BadRequest();
            }
            if(!getReceiver(ref receiver, order.emailReceiver))
            {
                return BadRequest(new { error = "recvNotExist" });
            }
            try
            {
                DirectMessageTitle dm = new DirectMessageTitle
                {
                    Sender = user,
                    receiver = receiver,
                    title = order.title
                };

                _context.Add(dm);
                _context.SaveChanges();

                await sendNotification(user, receiver, dm);

                return Ok(dm.id.ToString());
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
        /// Check if the sender and receiver are correct
        /// </summary>
        /// <param name="sender">The maker of the dm</param>
        /// <param name="recv">The receiver of the dm</param>
        /// <returns>True if both are correct, false otherwise</returns>
        private bool checkSenderAndReceiver(User sender, String recv)
        {
            bool sendToAdmin = recv == null;
            bool sentFromAdmin = AdminPolicy.isAdmin(sender, _context);

            if (sentFromAdmin && sendToAdmin)
            {
                return false;
            }
            if(!sentFromAdmin && !sendToAdmin)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the receiver of the dm
        /// </summary>
        /// <param name="receiver">A new user object, to save the receiver user object</param>
        /// <param name="mail">The email of the receiver</param>
        /// <returns>True if the receiver exists, false otherwise</returns>
        private bool getReceiver(ref User receiver, String mail)
        {
            User recv = new User();

            if(mail == null)
            {
                List<User> admins = _context.User.Where(u => u.role == RoleManager.getAdmin(_context)).ToList();

                Random rand = new Random();
                int index = rand.Next(admins.Count());
                recv = admins[index];
            }
            else
            {
                List<User> posibleUsers = _context.User.Where(u => u.email == mail).ToList();
                
                if(posibleUsers.Count() != 1)
                {
                    return false;
                }
                recv = posibleUsers.First();
            }
            receiver = recv;

            return true;
        }

        /// <summary>
        /// Sends the notification to the receiver
        /// </summary>
        /// <param name="user">The maker of the dm</param>
        /// <param name="recv">The receiver of the dm</param>
        /// <param name="title">The dm just created</param>
        private async Task sendNotification(User user, User recv, DirectMessageTitle title)
        {
            bool isAdmin = user.role == RoleManager.getAdmin(_context);

            NotificationType type = isAdmin ? NotificationType.OPEN_DM_FROM_ADMIN : NotificationType.OPEN_DM_FROM_USER;
            String recvName = isAdmin ? "" : recv.nickname;

            EmailSender.sendOpenCreateDMNotification(recv.email, recv.nickname, title.title);
            await SendNotification.send(_hub, recvName, recv, type, _context);
        }
    }
}
