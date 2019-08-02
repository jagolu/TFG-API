using System;
using System.Linq;
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
    public class BlockUserController : ControllerBase
    {
        private ApplicationDBContext _context;
        private IHubContext<NotificationHub> _hub;

        public BlockUserController(ApplicationDBContext context, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        [HttpPost]
        [Authorize]
        [ActionName("BlockUser")]
        public async Task<IActionResult> blockUser([FromBody] MakeAdmin_blockUser order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to kick the user from the group
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
            UserGroup targetUser = new UserGroup();
            Group group = new Group();

            if (!GroupAdminFuncionlities.checkFuncionality(user, ref group, order.groupName, ref targetUser, order.publicId, _context, GroupAdminFuncionlity.BLOCK_USER, order.make_unmake))
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

                await sendMessages(targetUser, group, order.make_unmake);

                return Ok(GroupPageManager.GetPage(user, group, _context));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        private async Task sendMessages(UserGroup targetUser, Group group, bool makeUnmake)
        {
            _context.Entry(targetUser).Reference("User").Load();
            //Send home news
            Home.Util.GroupNew.launch(targetUser.User, group, null, Home.Models.TypeGroupNew.BLOCK_USER_USER, makeUnmake, _context);
            Home.Util.GroupNew.launch(targetUser.User, group, null, Home.Models.TypeGroupNew.BLOCK_USER_GROUP, makeUnmake, _context);
            //Send notifications
            NotificationType typeNotification = makeUnmake ? NotificationType.BLOCKED : NotificationType.UNBLOCKED;
            await SendNotification.send(_hub, group.name, targetUser.User, typeNotification, _context);
        }
    }
}
