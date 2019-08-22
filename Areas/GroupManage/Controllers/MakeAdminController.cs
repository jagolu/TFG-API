using System;
using System.Threading.Tasks;
using API.Areas.Alive.Models;
using API.Areas.Alive.Util;
using API.Areas.GroupManage.Models;
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
    public class MakeAdminController : ControllerBase
    {
        private ApplicationDBContext _context;
        private IHubContext<NotificationHub> _hub;

        public MakeAdminController(ApplicationDBContext context, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        [HttpPost]
        [Authorize]
        [ActionName("MakeAdmin")]
        public async Task<IActionResult> makeAdminAsync([FromBody] MakeAdmin_blockUser order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to make admin to another user
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
            UserGroup targetUser = new UserGroup();
            Group group = new Group();

            if(!GroupAdminFuncionlities.checkFuncionality(user, ref group, order.groupName, ref targetUser, order.publicId, _context, GroupAdminFuncionality.MAKE_ADMIN, order.make_unmake))
            {
                return BadRequest();
            }
            if (!group.open) return BadRequest(new { error = "GroupBanned" });

            try
            {
                targetUser.role = order.make_unmake ? RoleManager.getGroupAdmin(_context) : RoleManager.getNormalUser(_context);
                _context.Update(targetUser);
                _context.SaveChanges();

                await sendNews(targetUser, group, order.make_unmake);

                return Ok(GroupPageManager.GetPage(user, group,  _context));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        private async Task sendNews(UserGroup target, Group group, bool makeUnmake)
        {
            //Send home news
            _context.Entry(target).Reference("User").Load();
            User targetUser = target.User;
            Home.Util.GroupNew.launch(targetUser, group, null, Home.Models.TypeGroupNew.MAKE_ADMIN_USER, makeUnmake, _context);
            Home.Util.GroupNew.launch(targetUser, group, null, Home.Models.TypeGroupNew.MAKE_ADMIN_GROUP, makeUnmake, _context);
            //Send notifications
            NotificationType typeNotification = makeUnmake ? NotificationType.MAKE_ADMIN : NotificationType.UNMAKE_ADMIN;
            await SendNotification.send(_hub, group.name, targetUser, typeNotification, _context);
        }
    }
}
