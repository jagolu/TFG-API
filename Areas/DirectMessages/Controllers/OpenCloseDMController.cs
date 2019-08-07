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
using Microsoft.Extensions.DependencyInjection;

namespace API.Areas.DirectMessages.Controllers
{
    [Route("DirectMessages/[action]")]
    [ApiController]
    public class OpenCloseDMController : ControllerBase
    {
        private ApplicationDBContext _context;
        private readonly IServiceScopeFactory scopeFactory;
        private IHubContext<NotificationHub> _hub;

        public OpenCloseDMController(ApplicationDBContext context, IServiceScopeFactory sf, IHubContext<NotificationHub> hub)
        {
            _context = context;
            scopeFactory = sf;
            _hub = hub;
        }

        [HttpGet]
        [Authorize]
        [ActionName("CloseDM")]
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

                using (var scope = scopeFactory.CreateScope())
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

        private bool checkAdminInDM(DirectMessageTitle title, User admin)
        {
            _context.Entry(title).Reference("receiver").Load();

            return title.senderid == admin.id || title.receiver.id == admin.id;
        }

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

        private async Task sendMailAndNotification(DirectMessageTitle title, User caller, bool open)
        {
            _context.Entry(title).Reference("Sender").Load();
            _context.Entry(title).Reference("receiver").Load();
            User theUser = title.Sender.id == caller.id ? title.Sender : title.receiver;
            User notificationReceiver = title.Sender.id == caller.id ? title.receiver : title.Sender;

            EmailSender.sendOpenCloseDMNotification(theUser.email, theUser.nickname, title.title, open);

            await sendNotification(notificationReceiver, open);
        }

        private async Task sendNotification(User recv, bool open)
        {

            NotificationType type = open ? NotificationType.REOPEN_DM : NotificationType.CLOSE_DM;

            await SendNotification.send(_hub, "", recv, type, _context);
        }
    }
}
