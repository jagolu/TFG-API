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
        private ApplicationDBContext _context;

        public StandNewsController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ActionName("StandHome")]
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
