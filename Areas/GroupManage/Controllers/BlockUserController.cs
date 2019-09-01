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
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The database context of the application</value>
        private ApplicationDBContext _context;

        /// <value>The notifications hub</value>
        private IHubContext<NotificationHub> _notificationsHub;
        
        /// <value>The chat hub</value>
        private IHubContext<ChatHub> _chatHub;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="notificationHub">The notifications hub</param>
        /// <param name="chatHub">The chat hub</param>
        public BlockUserController(ApplicationDBContext context, IHubContext<NotificationHub> notificationHub, IHubContext<ChatHub> chatHub)
        {
            _context = context;
            _notificationsHub = notificationHub;
            _chatHub = chatHub;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        [HttpPost]
        [Authorize]
        [ActionName("BlockUser")]
        /// <summary>
        /// Block/unblock a user from the group
        /// </summary>
        /// <param name="order">The info to block/unblock the user</param>
        /// See <see cref="Areas.GroupManage.Models.MakeAdmin_blockUser"/> to know the param structure
        /// <returns>The updated group page</returns>
        /// See <see cref="Areas.GroupManage.Models.GroupPage"/> to know the response structure
        public async Task<IActionResult> blockUser([FromBody] MakeAdmin_blockUser order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to kick the user from the group
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
            UserGroup targetUser = new UserGroup();
            Group group = new Group();

            if (!GroupAdminFuncionlities.checkFuncionality(user, ref group, order.groupName, ref targetUser, order.publicId, _context, GroupAdminFuncionality.BLOCK_USER, order.make_unmake))
            {
                return BadRequest();
            }
            if (!group.open) return BadRequest(new { error = "GroupBanned" });

            try
            {
                _context.Entry(user).Collection("groups").Load();
                UserGroup callerUG = user.groups.Where(g => g.groupid == group.id).First();

                targetUser.blocked = !targetUser.blocked;
                targetUser.role = RoleManager.getGroupNormal(_context);
                targetUser.blockedBy = callerUG.role;
                _context.Update(targetUser);
                _context.SaveChanges();

                _context.Entry(targetUser).Reference("User").Load();
                if(order.make_unmake) await KickChatNotification.sendKickMessageAsync(group.name, targetUser.User.publicid, _chatHub);
                await sendMessages(targetUser, group, order.make_unmake);

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
        /// Send the news to group and user and the notification to the user
        /// </summary>
        /// <param name="targetUser">The member of the group that has been blocked/unblocked</param>
        /// <param name="group">The group where the member has been blocked/unblocked</param>
        /// <param name="makeUnmake">True if the user has been blocked, false otherwise</param>
        private async Task sendMessages(UserGroup targetUser, Group group, bool makeUnmake)
        {
            _context.Entry(targetUser).Reference("User").Load();
            //Send home news
            Home.Util.GroupNew.launch(targetUser.User, group, null, Home.Models.TypeGroupNew.BLOCK_USER_USER, makeUnmake, _context);
            Home.Util.GroupNew.launch(targetUser.User, group, null, Home.Models.TypeGroupNew.BLOCK_USER_GROUP, makeUnmake, _context);
            //Send notifications
            NotificationType typeNotification = makeUnmake ? NotificationType.BLOCKED : NotificationType.UNBLOCKED;
            await SendNotification.send(_notificationsHub, group.name, targetUser.User, typeNotification, _context);
        }
    }
}
