using System;
using System.Collections.Generic;
using API.Areas.Home.Models;
using API.Areas.Home.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Home.Controllers
{
    [Route("Home/[action]")]
    [ApiController]
    public class StandNewsController : ControllerBase
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
        public StandNewsController(ApplicationDBContext context)
        {
            _context = context;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        [HttpGet]
        [ActionName("StandHome")]
        /// <summary>
        /// Get the news for not logged users
        /// </summary>
        /// <returns>The IActionResult of the get the not logged news action</returns>
        /// See <see cref="Areas.Home.Models.NewMessage"/> to see the response structure
        public IActionResult GetStand()
        {
            bool isAdmin;
            try
            {
                User user = TokenUserManager.getUserFromToken(HttpContext, _context);
                isAdmin = AdminPolicy.isAdmin(user, _context);
            }
            catch (Exception)
            {
                isAdmin = false;
            }
            try
            {
                List<NewMessage> retMessage = GetNews.getStandNews(isAdmin, _context);

                return Ok(retMessage);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
