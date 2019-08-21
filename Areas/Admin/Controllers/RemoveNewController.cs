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
        private ApplicationDBContext _context;
        private readonly IServiceScopeFactory _scopeFactory;

        public RemoveNewController(ApplicationDBContext context, IServiceScopeFactory sf)
        {
            _context = context;
            _scopeFactory = sf;
        }

        [HttpGet]
        [Authorize]
        [ActionName("RemoveNew")]
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
