using System;
using API.Areas.GroupManage.Models;
using API.Areas.GroupManage.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.GroupManage.Controllers
{
    [Route("Group/[action]")]
    [ApiController]
    public class RemoveUserFromGroupController : ControllerBase
    {
        private ApplicationDBContext _context;

        public RemoveUserFromGroupController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("RemoveUser")]
        public IActionResult removeUser([FromBody] KickUser order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to kick the user from the group
            UserGroup targetUser = new UserGroup();

            if (!GroupUserManager.CheckUserGroup(user, order.groupName, ref targetUser, order.publicId, _context, TypeCheckGroupUser.REMOVE_USER, false))
            {
                return BadRequest(new { error = "" });
            }

            try
            {
                _context.UserGroup.Remove(targetUser);
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
