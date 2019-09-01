using System;
using API.Areas.Alive.Util;
using API.Areas.GroupManage.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace API.Areas.GroupManage.Controllers
{
    [Route("Group/[action]")]
    [ApiController]
    public class RemoveGroupController : ControllerBase
    {
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The database context of the application</value>
        private ApplicationDBContext _context;

        /// <value>The notifications hub</value>
        private IHubContext<NotificationHub> _hub;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="hub">The notifications hub</param>
        public RemoveGroupController(ApplicationDBContext context, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        [HttpPost]
        [Authorize]
        [ActionName("RemoveGroup")]
        /// <summary>
        /// Deletes a group
        /// </summary>
        /// <param name="order">The info to remove a group</param>
        /// See <see cref="Areas.GroupManage.Models.RemoveGroup"/> to know the param structure
        /// <returns>IActionResult of the remove group action</returns>
        public IActionResult removeGroup([FromBody] Models.RemoveGroup order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to kick the user from the group
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
            Group group = new Group();

            if (!GroupMakerFuncionlities.checkFuncionality(user, ref group, order.name, GroupMakerFuncionality.REMOVE_GROUP, _context))
            {
                return BadRequest();
            }
            if (!PasswordHasher.areEquals(order.userPassword, user.password))
            {
                return BadRequest(new { error = "IncorrectOldPassword" });
            }
            if (!group.open) return BadRequest(new { error = "GroupBanned" });
            try
            {
                RemoveGroup.remove(group, _context, _hub);

                return Ok(new { success = "SuccesfullGroupRemoved" });
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
