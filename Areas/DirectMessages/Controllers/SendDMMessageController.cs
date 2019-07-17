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
    public class SendDMMessageController : ControllerBase
    {
        private ApplicationDBContext _context;
        private readonly IServiceScopeFactory scopeFactory;

        public SendDMMessageController(ApplicationDBContext context, IServiceScopeFactory sf)
        {
            _context = context;
            scopeFactory = sf;
        }

        [HttpPost]
        [Authorize]
        [ActionName("SendDMMessage")]
        public IActionResult sendMsg([FromBody] SendDMMessage order)
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

                if (isAdmin)
                {
                    //TODO  send email al usuario avisandole
                }

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

        private bool checkOrder(ref DirectMessageTitle title, string dmId, User user)
        {
            _context.Entry(user).Collection("directMessages").Load();
            List<DirectMessageTitle> dms = user.directMessages.Where(dm => dm.id.ToString() == dmId).ToList();
            dms.AddRange(_context.DirectMessagesTitle.Where(dm => dm.id.ToString() == dmId && dm.Receiver == user).ToList());
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
    }
}
