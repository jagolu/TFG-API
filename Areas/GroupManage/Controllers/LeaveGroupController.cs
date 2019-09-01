using API.Areas.Alive.Util;
using API.Areas.GroupManage.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace API.Areas.GroupManage.Controllers
{
    [Route("Group/[action]")]
    [ApiController]
    public class LeaveGroupController : ControllerBase
    {
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The database context of the application</value>
        private ApplicationDBContext _context;

        /// <value>The scope factory to get an updated database context</value>
        private readonly IServiceScopeFactory _scopeFactory;

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
        /// <param name="sf">The service factory</param>
        /// <param name="hub">The notifications hub</param>
        public LeaveGroupController(ApplicationDBContext context, IServiceScopeFactory sf, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _scopeFactory = sf;
            _hub = hub;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        [HttpGet]
        [Authorize]
        [ActionName("LeaveGroup")]
        /// <summary>
        /// Get a user out of a group
        /// </summary>
        /// <param name="groupName">The name of the group that the user is gonna left</param>
        /// <returns>IActionResult of the leaving group action</returns>
        public async System.Threading.Tasks.Task<IActionResult> leaveGroup(string groupName)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to leave the group
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
            UserGroup ugCaller = new UserGroup();
            Group group = new Group();

            if(!UserFromGroup.isOnIt(user.id, ref group, groupName, ref ugCaller, _context))
            {
                return BadRequest();
            }
            if (!group.open) return BadRequest(new { error = "GroupBanned" });
            if (!await QuitUserFromGroup.quitUser(ugCaller, _context, _hub))
            {
                return StatusCode(500);
            }
            InteractionManager.manageInteraction(user, group, interactionType.LEAVED, _context);

            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                var groupE = dbContext.Group.Where(g => g.name == groupName);

                if(groupE.Count() == 1)  Home.Util.GroupNew.launch(user, group, null, Home.Models.TypeGroupNew.JOIN_LEFT_GROUP, false, _context);
                if(groupE.Count() == 1)  Home.Util.GroupNew.launch(user, group, null, Home.Models.TypeGroupNew.JOIN_LEFT_USER, false, _context);
            }

            return Ok(new { success="SuccesfullGroupLeave"});
        }
    }
}
