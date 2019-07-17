using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.DirectMessages.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Areas.DirectMessages.Controllers
{
    [Route("DirectMessages/[action]")]
    [ApiController]
    public class OpenCloseDMController : ControllerBase
    {
        private ApplicationDBContext _context;
        private readonly IServiceScopeFactory scopeFactory;

        public OpenCloseDMController(ApplicationDBContext context, IServiceScopeFactory sf)
        {
            _context = context;
            scopeFactory = sf;
        }

        [HttpGet]
        [Authorize]
        [ActionName("CloseDM")]
        public IActionResult close(string id, string openOrder)
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
                sendMail(title, user, open);

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
            _context.Entry(title).Reference("Receiver").Load();

            return title.senderId == admin.id || title.Receiver.id == admin.id;
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

        private void sendMail(DirectMessageTitle title, User caller, bool open)
        {
            _context.Entry(title).Reference("Sender").Load();
            _context.Entry(title).Reference("Receiver").Load();
            User theUser = new User();
            if (title.Sender.id == caller.id) theUser = title.Sender;
            else theUser = title.Receiver;

            EmailSender.sendOpenCloseDMNotification(theUser.email, theUser.nickname, title.title, open);
        }
    }
}
