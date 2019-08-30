using System;
using System.Collections.Generic;
using API.Areas.Admin.Models;
using API.Areas.Home.Models;
using API.Areas.Home.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Admin.Controllers
{
    [Route("Admin/[action]")]
    [ApiController]
    public class LaunchNewController : ControllerBase
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
        public LaunchNewController(ApplicationDBContext context)
        {
            _context = context;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //
        
        [HttpPost]
        [Authorize]
        [ActionName("LaunchNew")]
        /// <summary>
        /// Launchs a new new for all the users
        /// </summary>
        /// <param name="message">The info</param>
        /// See <see cref="Areas.Admin.Models.Message"/> to see the param info
        /// <returns>The IActionResult of the launch new action</returns>
        public IActionResult launchNew([FromBody] Message message)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");

            try
            {
                _context.Add(new New
                {
                    title = "Aviso de los administradores!",
                    message = message.message
                });

                _context.SaveChanges();
                List<NewMessage> retMessage = GetNews.getStandNews(true, _context);

                return Ok(retMessage);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
