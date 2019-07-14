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
    public class MakeAdminController : ControllerBase
    {
        private ApplicationDBContext _context;

        public MakeAdminController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("MakeAdmin")]
        public IActionResult makeAdmin([FromBody] MakeAdmin_blockUser order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to make admin to another user
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
            UserGroup targetUser = new UserGroup();
            Group group = new Group();

            if(!GroupUserManager.CheckUserGroup(user, ref group, order.groupName, ref targetUser, order.publicId, _context, TypeCheckGroupUser.MAKE_ADMIN, order.make_unmake))
            {
                return BadRequest();
            }
            if (!group.open) return BadRequest(new { error = "GroupBanned" });

            try
            {
                targetUser.role = order.make_unmake ? RoleManager.getGroupAdmin(_context) : RoleManager.getNormalUser(_context);
                _context.Update(targetUser);
                _context.SaveChanges();

                _context.Entry(targetUser).Reference("User").Load();
                Home.Util.GroupNew.launch(targetUser.User, group, Home.Models.TypeGroupNew.MAKE_ADMIN_USER, order.make_unmake, _context);
                Home.Util.GroupNew.launch(targetUser.User, group, Home.Models.TypeGroupNew.MAKE_ADMIN_GROUP, order.make_unmake, _context);

                return Ok(GroupPageManager.GetPage(user, group,  _context));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
