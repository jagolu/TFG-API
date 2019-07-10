using API.Areas.GroupManage.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public LeaveGroupController(ApplicationDBContext context, IServiceScopeFactory sf)
        {
            _context = context;
            scopeFactory = sf;
        }

        [HttpGet]
        [Authorize]
        [ActionName("LeaveGroup")]
        public IActionResult leaveGroup(string groupName)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to leave the group
            UserGroup ugCaller = new UserGroup();
            Group group = new Group();

            if(!UserInGroup.checkUserInGroup(user.id, ref group, groupName, ref ugCaller, _context))
            {
                return BadRequest();
            }
            if (!QuitUserFromGroup.quitUser(ugCaller, _context))
            {
                return StatusCode(500);
            }

            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                var groupE = dbContext.Group.Where(g => g.name == groupName);

                if(groupE.Count() == 1)  Common.Util.GroupNew.launch(user, group, Common.Models.TypeGroupNew.JOIN_LEFT, false, _context);
            }

            return Ok(new { success="SuccesfullGroupLeave"});
        }
    }
}
