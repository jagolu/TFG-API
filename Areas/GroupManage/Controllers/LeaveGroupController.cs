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
        private ApplicationDBContext _context;
        private readonly IServiceScopeFactory scopeFactory;
        private IHubContext<NotificationHub> _hub;

        public LeaveGroupController(ApplicationDBContext context, IServiceScopeFactory sf, IHubContext<NotificationHub> hub)
        {
            _context = context;
            scopeFactory = sf;
            _hub = hub;
        }

        [HttpGet]
        [Authorize]
        [ActionName("LeaveGroup")]
        public IActionResult leaveGroup(string groupName)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to leave the group
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
            UserGroup ugCaller = new UserGroup();
            Group group = new Group();

            if(!UserInGroup.checkUserInGroup(user.id, ref group, groupName, ref ugCaller, _context))
            {
                return BadRequest();
            }
            if (!group.open) return BadRequest(new { error = "GroupBanned" });
            if (!QuitUserFromGroup.quitUser(ugCaller, _context, _hub))
            {
                return StatusCode(500);
            }

            using (var scope = scopeFactory.CreateScope())
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
