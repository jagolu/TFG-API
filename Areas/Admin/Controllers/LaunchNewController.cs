using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.Admin.Models;
using API.Areas.Home.Models;
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
        private ApplicationDBContext _context;

        public LaunchNewController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("LaunchNew")]
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

                List<New> news = _context.News.Where(n => n.userId == user.id || n.userId == null).OrderByDescending(nn => nn.date).ToList();
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
