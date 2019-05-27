using System;
using System.Linq;
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
    public class BlockUserController : ControllerBase
    {
        private ApplicationDBContext _context;

        public BlockUserController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("BlockUser")]
        public IActionResult blockUser([FromBody] MakeAdmin_blockUser order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to kick the user from the group
            UserGroup targetUser = new UserGroup();
            Group group = new Group();

            if (!GroupUserManager.CheckUserGroup(user, ref group, order.groupName, ref targetUser, order.publicId, _context, TypeCheckGroupUser.BLOCK_USER, order.make_unmake))
            {
                return BadRequest();
            }

            try
            {
                _context.Entry(user).Collection("groups").Load();
                UserGroup callerUG = user.groups.Where(g => g.groupId == group.id).First();

                targetUser.blocked = !targetUser.blocked;
                targetUser.role = _context.Role.Where(r => r.name == "GROUP_NORMAL").First();
                targetUser.blockedBy = callerUG.role;
                _context.Update(targetUser);
                _context.SaveChanges();

                return Ok(GroupPageManager.GetPage(user, group, _context));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
