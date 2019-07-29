using API.Areas.GroupManage.Models;
using API.Areas.GroupManage.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace API.Areas.GroupManage.Controllers
{
    [Route("Group/[action]")]
    [ApiController]
    public class ChangeWeekPayController : ControllerBase
    {
        private ApplicationDBContext _context;

        public ChangeWeekPayController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("ManageWeekPay")]
        public IActionResult manageWeeklyPay([FromBody] ManageWeeklyPay order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to make admin to another user
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
            Group group = new Group();

            if (!CallerInGroup.CheckUserCapabilities(user, ref group, order.groupName, TypeCheckCapabilites.MANAGEWEEKPAY, _context, "", ""))
            {
                return BadRequest();
            }
            if (order.weeklyPay<100 || order.weeklyPay>2000)
            {
                return BadRequest();
            }
            if (!group.open) return BadRequest(new { error = "GroupBanned" });

            try
            {
                int oldPay = group.weeklyPay;
                group.weeklyPay = order.weeklyPay;
                _context.Update(group);
                _context.SaveChanges();

                launchNews_changeUserCoins(group, oldPay, order.weeklyPay, user.id);
                _context.SaveChanges();

                return Ok(GroupPageManager.GetPage(user, group, _context));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        private void launchNews_changeUserCoins(Group group, int oldPay, int newPay, Guid callerId)
        {
            _context.Entry(group).Collection("users").Load();
            int minus = oldPay - newPay;

            Home.Util.GroupNew.launch(null, group, null, Home.Models.TypeGroupNew.CHANGE_WEEKLYPAY_GROUP, false, _context);
            group.users.ToList().ForEach(u =>
            {
                bool isMaker = u.userId == callerId;
                u.coins -= minus;
                Home.Util.GroupNew.launch(null, group, null, Home.Models.TypeGroupNew.CHANGE_WEEKLYPAY_USER, isMaker, _context);
            });
        }
    }
}
