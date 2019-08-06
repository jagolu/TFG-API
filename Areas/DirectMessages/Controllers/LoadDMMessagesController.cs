using System;
using System.Collections.Generic;
using System.Linq;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.DirectMessages.Controllers
{
    [Route("DirectMessages/[action]")]
    [ApiController]
    public class LoadDMMessagesController : ControllerBase
    {
        private ApplicationDBContext _context;

        public LoadDMMessagesController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("LoadDMMessages")]
        public IActionResult load(string dmId)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            DirectMessageTitle title = new DirectMessageTitle();
            if (!user.open) return BadRequest(new { error = "YoureBanned" });

            if(!getDMTitle(ref title, dmId, user))
            {
                return BadRequest();
            }

            try
            {
                readMessages(title, AdminPolicy.isAdmin(user, _context));
                _context.SaveChanges();
                Models.DMRoom room = new Models.DMRoom(title, user, _context);
                return Ok(room);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        private bool getDMTitle(ref DirectMessageTitle title, string dmId, User user)
        {
            _context.Entry(user).Collection("directMessages").Load();
            List<DirectMessageTitle> dms = user.directMessages.Where(dm => dm.id.ToString() == dmId).ToList();
            dms.AddRange(_context.DirectMessagesTitle.Where(dm => dm.id.ToString() == dmId && dm.receiver == user).ToList());
            if (dms.Count() != 1)
            {
                return false;
            }

            title = dms.First();

            return true;
        }

        private void readMessages(DirectMessageTitle title, bool isAdmin)
        {
            if (isAdmin)
            {
                title.unreadMessagesForAdmin = 0;
            }
            else
            {
                title.unreadMessagesForUser = 0;
            }
        }
    }
}
