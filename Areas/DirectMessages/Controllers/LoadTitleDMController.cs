using System;
using API.Data;
using API.Data.Models;
using API.Areas.DirectMessages.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.DirectMessages.Controllers
{
    [Route("DirectMessages/[action]")]
    [ApiController]
    public class LoadTitleDMController : ControllerBase
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
        public LoadTitleDMController(ApplicationDBContext context)
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
        [ActionName("LoadDMTitles")]
        /// <summary>
        /// Get all the dm conversations of the user
        /// </summary>
        /// <returns>The IActionResult of the load titles action</returns>
        /// See <see cref="Areas.DirectMessages.Models.DMTitle"/> to know the response structure
        public IActionResult loadTitles()
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!user.open) return BadRequest(new { error = "YoureBanned" });

            try
            {
                List<DMTitle> retList = new List<DMTitle>();

                _context.Entry(user).Collection("directMessages").Load();
                bool isAdmin = API.Util.AdminPolicy.isAdmin(user, _context);
                List<DirectMessageTitle> all = new List<DirectMessageTitle>();

                all.AddRange(user.directMessages);
                all.AddRange(_context.DirectMessagesTitle.Where(dm => dm.receiver == user).ToList());

                unreadTitles(all, isAdmin).ForEach(dm => retList.Add(new DMTitle(dm, user.id, isAdmin, _context)));
                unclosedTitles(all, isAdmin).ForEach(dm => retList.Add(new DMTitle(dm, user.id, isAdmin, _context)));
                closedTitles(all).ForEach(dm => retList.Add(new DMTitle(dm, user.id, isAdmin, _context)));
                
                return Ok(retList);
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
        /// Get the unread dm conversations 
        /// </summary>
        /// <param name="msgs">All the dm conversations of the user</param>
        /// <param name="isAdmin">True if the caller is an admin, false otherwise</param>
        /// <returns>The list of unread dm conversations</returns>
        private static List<DirectMessageTitle> unreadTitles(List<DirectMessageTitle> msgs, bool isAdmin)
        {
            if (isAdmin)
            {
                return msgs.Where(dm => dm.unreadMessagesForAdmin > 0 && !dm.closed).OrderByDescending(dm=> dm.lastUpdate).ToList();
            }

            return msgs.Where(dm => dm.unreadMessagesForUser > 0 && !dm.closed).OrderByDescending(dm => dm.lastUpdate).ToList();
        }

        /// <summary>
        /// Get the unclosed dm conversations 
        /// </summary>
        /// <param name="msgs">All the dm conversations of the user</param>
        /// <param name="isAdmin">True if the caller is an admin, false otherwise</param>
        /// <returns>The list of unclosed dm conversations</returns>
        private static List<DirectMessageTitle> unclosedTitles(List<DirectMessageTitle> msgs, bool isAdmin)
        {
            if (isAdmin)
            {
                return msgs.Where(dm => dm.unreadMessagesForAdmin == 0 && !dm.closed).OrderByDescending(dm => dm.lastUpdate).ToList();
            }

            return msgs.Where(dm => dm.unreadMessagesForUser == 0 && !dm.closed).OrderByDescending(dm => dm.lastUpdate).ToList();
        }

        /// <summary>
        /// Get the closed dm conversations 
        /// </summary>
        /// <param name="msgs">All the dm conversations of the user</param>
        /// <param name="isAdmin">True if the caller is an admin, false otherwise</param>
        /// <returns>The list of closed dm conversations</returns>
        private static List<DirectMessageTitle> closedTitles(List<DirectMessageTitle> msgs)
        {
            return msgs.Where(dm => dm.closed).OrderByDescending(dm => dm.lastUpdate).ToList();
        }
    }
}
