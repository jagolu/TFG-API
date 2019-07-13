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
    public class BanUserController : ControllerBase
    {
        private ApplicationDBContext _context;

        public BanUserController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("BanUser")]
        public IActionResult banUser([FromBody] BanUser order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            User targetUser = new User();
            if (!AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
                        
            if(!existUser(ref targetUser, order.publicUserId))
            {
                return BadRequest();
            }
            if(validOrder(targetUser, order.ban))
            {
                return BadRequest();
            }

            try
            {
                targetUser.open = !targetUser.open;
                _context.Update(targetUser);
                _context.SaveChanges();

                EmailSender.sendBanNotification(targetUser.email, targetUser.nickname, order.ban);

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest(500);
            }
        }

        public bool existUser(ref User user, string publicUserId)
        {
            List<User> existUser = _context.User.Where(u => u.publicId == publicUserId).ToList();
            if (existUser.Count() != 1)
            {
                return false;
            }

            user = existUser.First();

            return true;
        }

        public bool validOrder(User user, bool order)
        {
            bool userBlock = user.open;

            return userBlock != order;
        }
    }
}
