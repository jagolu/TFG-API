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
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
            UserGroup targetUser = new UserGroup();
            Group group = new Group();

            if (!GroupUserManager.CheckUserGroup(user, ref group, order.groupName, ref targetUser, order.publicId, _context, TypeCheckGroupUser.BLOCK_USER, order.make_unmake))
            {
                return BadRequest();
            }
            if (!group.open) return BadRequest(new { error = "GroupBanned" });

            try
            {
                _context.Entry(user).Collection("groups").Load();
                UserGroup callerUG = user.groups.Where(g => g.groupId == group.id).First();

                targetUser.blocked = !targetUser.blocked;
                targetUser.role = RoleManager.getGroupNormal(_context);
                targetUser.blockedBy = callerUG.role;
                _context.Update(targetUser);
                _context.SaveChanges();

                _context.Entry(targetUser).Reference("User").Load();
                Home.Util.GroupNew.launch(targetUser.User, group, Home.Models.TypeGroupNew.BLOCK_USER_USER, order.make_unmake, _context);
                Home.Util.GroupNew.launch(targetUser.User, group, Home.Models.TypeGroupNew.BLOCK_USER_GROUP, order.make_unmake, _context);

                return Ok(GroupPageManager.GetPage(user, group, _context));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
