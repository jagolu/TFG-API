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
        public MakeAdminController(ApplicationDBContext context, IHubContext<NotificationHub> hub)
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
        [ActionName("MakeAdmin")]
        /// <summary>
        /// Gives the group-admin to a member of a group
        /// </summary>
        /// <param name="order">The infoto make a user admin of a group</param>
        /// See <see cref="Areas.GroupManage.Models.MakeAdmin_blockUser"/> to know the param structure
        /// <returns>The updated group page</returns>
        /// See <see cref="Areas.GroupManage.Models.GroupPage"/> to know the response structure
        public async Task<IActionResult> makeAdmin([FromBody] MakeAdmin_blockUser order)
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


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Send the news and the notifications to the group and the new admin
        /// </summary>
        /// <param name="target"></param>
        /// <param name="group"></param>
        /// <param name="makeUnmake"></param>
        /// <returns></returns>
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
