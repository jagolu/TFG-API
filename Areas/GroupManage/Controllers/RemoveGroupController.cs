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
        private ApplicationDBContext _context;
        private IHubContext<NotificationHub> _hub;

        public RemoveGroupController(ApplicationDBContext context, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        [HttpPost]
        [Authorize]
        [ActionName("RemoveGroup")]
        public IActionResult removeGroup([FromBody] Models.RemoveGroup order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to kick the user from the group
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
            Group group = new Group();

            if (!CallerInGroup.CheckUserCapabilities(user, ref group, order.name, TypeCheckCapabilites.REMOVE_GROUP, _context))
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
                RemoveGroup.Remove(group, _context, _hub);

                return Ok(new { success = "SuccesfullGroupRemoved" });
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
