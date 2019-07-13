using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.Home.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Areas.Home.Controllers
{
    [Route("Home/[action]")]
    [ApiController]
    public class HomeNewsController : ControllerBase
    {
        private ApplicationDBContext _context;

        public HomeNewsController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("AuthHome")]
        public IActionResult getAuth()
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");

            try
            {
                List<New> news = _context.News.Where(n => n.userId == user.id || n.userId == null).OrderByDescending(nn => nn.date).ToList();
                List<NewMessage> retMessage = addNews(news);

                return Ok(retMessage);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }


        [HttpGet]
        [ActionName("StandHome")]
        public IActionResult GetStand()
        {
            try
            {
                List<New> news = _context.News.Where(n => n.userId == null).OrderByDescending(nn => nn.date).ToList();
                List<NewMessage> retMessage = addNews(news);

                return Ok(retMessage);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }



        private List<NewMessage> addNews(List<New> news)
        {
            List<NewMessage> retMessage = new List<NewMessage>();
            news.ForEach(n => retMessage.Add(new NewMessage(n)));

            return retMessage;
        }
    }
}
