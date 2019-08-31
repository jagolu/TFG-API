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
        /// <param name="context">The database context</param>
        public LoadDMMessagesController(ApplicationDBContext context)
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
        [ActionName("LoadDMMessages")]
        /// <summary>
        /// Load the messages in a dm conversation
        /// </summary>
        /// <param name="dmId">The id of the dm conversation</param>
        /// <returns>IActionResult of the load action</returns>
        /// See <see cref="Areas.DirectMessages.Models.DMRoom"/> to know the response structure
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


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Get the info of the dm conversation
        /// </summary>
        /// <param name="title">A new DirectMessageTitle object to save the dm conversation on it</param>
        /// <param name="dmId">The id of the dm conversation</param>
        /// <param name="user">The user who wants to load the conversation</param>
        /// <returns>true if the dm conversation exists, false otherwise</returns>
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

        /// <summary>
        /// Mark as read the messages for the user
        /// </summary>
        /// <param name="title">The dm conversation</param>
        /// <param name="isAdmin">True if the caller is an admin, false otherwise</param>
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
