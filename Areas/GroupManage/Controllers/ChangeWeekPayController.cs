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
        public ChangeWeekPayController(ApplicationDBContext context)
        {
            _context = context;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        [HttpPost]
        [Authorize]
        [ActionName("ManageWeekPay")]
        /// <summary>
        /// Change the weekly pay of a group
        /// </summary>
        /// <param name="order">The info to change the weekly pay</param>
        /// See <see cref="Areas.GroupManage.Models.ManageWeeklyPay"/> to know the param structure
        /// <returns>The updated group page</returns>
        /// See <see cref="Areas.GroupManage.Models.GroupPage"/> to know the response structure
        public IActionResult manageWeeklyPay([FromBody] ManageWeeklyPay order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to make admin to another user
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
            Group group = new Group();

            if (!GroupMakerFuncionlities.checkFuncionality(user, ref group, order.groupName, GroupMakerFuncionality.MANAGEWEEKPAY, _context, "", ""))
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


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Send the news to the group
        /// </summary>
        /// <param name="group">The group to send the news</param>
        /// <param name="oldPay">The old weekly pay in the group</param>
        /// <param name="newPay">The new weekly pay in the group</param>
        /// <param name="callerId">The id of the maker of the group</param>
        private void launchNews_changeUserCoins(Group group, int oldPay, int newPay, Guid callerId)
        {
            _context.Entry(group).Collection("users").Load();
            int minus = oldPay - newPay;

            Home.Util.GroupNew.launch(null, group, null, Home.Models.TypeGroupNew.CHANGE_WEEKLYPAY_GROUP, false, _context);
            group.users.ToList().ForEach(u =>
            {
                _context.Entry(u).Reference("User").Load();
                bool isMaker = u.userid == callerId;
                u.coins -= minus;
                
                Home.Util.GroupNew.launch(u.User, group, null, Home.Models.TypeGroupNew.CHANGE_WEEKLYPAY_USER, isMaker, _context);
            });
        }
    }
}
