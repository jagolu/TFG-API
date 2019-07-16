using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.Admin.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Admin.Controllers
{
    [Route("Admin/[action]")]
    [ApiController]
    public class BanGroupController : ControllerBase
    {
        private ApplicationDBContext _context;

        public BanGroupController(ApplicationDBContext context)
        {
            _context = context;
        }


        [HttpPost]
        [Authorize]
        [ActionName("BanGroup")]
        public IActionResult banUser([FromBody] BanGroup order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            Group targetGroup = new Group();
            if (!AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");

            if (!existGroup(ref targetGroup, order.groupName))
            {
                return BadRequest();
            }
            if (validOrder(targetGroup, order.ban))
            {
                return BadRequest();
            }

            try
            {
                targetGroup.open = !targetGroup.open;
                _context.Update(targetGroup);
                _context.SaveChanges();

                sendNews(targetGroup, order.ban);

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest(500);
            }
        }

        private bool existGroup(ref Group group, string name)
        {
            List<Group> existGroup = _context.Group.Where(u => u.name == name).ToList();
            if (existGroup.Count() != 1)
            {
                return false;
            }

            group = existGroup.First();

            return true;
        }

        private bool validOrder(Group group, bool order)
        {
            bool userBlock = group.open;

            return userBlock != order;
        }

        private void sendNews(Group group, bool ban)
        {
            _context.Entry(group).Collection("users").Load();

            group.users.ToList().ForEach(u =>
            {
                _context.Entry(u).Reference("User").Load();
                Home.Util.GroupNew.launch(u.User, group, null, Home.Models.TypeGroupNew.BAN_GROUP, ban, _context);
            });
        }
    }
}
