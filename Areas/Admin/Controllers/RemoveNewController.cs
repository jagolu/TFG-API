using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.Home.Models;
using API.Areas.Home.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Areas.Admin.Controllers
{
    [Route("Admin/[action]")]
    [ApiController]
    public class RemoveNewController : ControllerBase
    {
        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //
        
        /// <value>The database context of the application</value>
        private ApplicationDBContext _context;

        /// <value>A scope factory to reset the database context</value>
        private readonly IServiceScopeFactory _scopeFactory;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="sf">The Scope Factory</param>
        public RemoveNewController(ApplicationDBContext context, IServiceScopeFactory sf)
        {
            _context = context;
            _scopeFactory = sf;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        
        [HttpGet]
        [Authorize]
        [ActionName("RemoveNew")]
        /// <summary>
        /// Removes a new
        /// </summary>
        /// <param name="id">The id of the new</param>
        /// <returns>The IActionResult of the remove new action</returns>
        public IActionResult removeNew(string id)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");

            List<New> possibleNews = _context.News.Where(n => n.id.ToString() == id && n.userid == null).ToList();
            if(possibleNews.Count() != 1)
            {
                return BadRequest();
            }

            try
            {
                _context.Remove(possibleNews.First());
                _context.SaveChanges();

                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                    List<NewMessage> retMessage = GetNews.getStandNews(true, dbContext);
                    return Ok(retMessage);
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
