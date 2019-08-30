using System;
using System.Collections.Generic;
using API.Areas.Home.Models;
using API.Areas.Home.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Home.Controllers
{
    [Route("Home/[action]")]
    [ApiController]
    public class AuthorizedNewsController : ControllerBase
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
        public AuthorizedNewsController(ApplicationDBContext context)
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
        [ActionName("AuthHome")]
        /// <summary>
        /// Get the news for auhtorized users
        /// </summary>
        /// <returns>The IActionResult of the get authorized news action</returns>
        /// See <see cref="Areas.Home.Models.NewMessage"/> to see the response structure
        public IActionResult getAuth()
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");

            try
            {
                List<NewMessage> retMessage = GetNews.getAuthNews(user.id, _context);

                return Ok(retMessage);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}