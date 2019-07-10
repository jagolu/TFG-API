using System;
using System.Linq;
using API.Areas.GroupManage.Models;
using API.Areas.GroupManage.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Areas.GroupManage.Controllers
{
    [Route("Group/[action]")]
    [ApiController]
    public class RemoveUserFromGroupController : ControllerBase
    {
        private ApplicationDBContext _context;
        private readonly IServiceScopeFactory scopeFactory;

        public RemoveUserFromGroupController(ApplicationDBContext context, IServiceScopeFactory sf)
        {
            _context = context;
            scopeFactory = sf;
        }

        [HttpPost]
        [Authorize]
        [ActionName("RemoveUser")]
        public IActionResult removeUser([FromBody] KickUser order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to kick the user from the group
            UserGroup targetUser = new UserGroup();
            Group group = new Group();

            if (!GroupUserManager.CheckUserGroup(user, ref group, order.groupName, ref targetUser, order.publicId, _context, TypeCheckGroupUser.REMOVE_USER, false))
            {
                return BadRequest();
            }

            try
            {
                _context.Entry(targetUser).Reference("User").Load();
                User sendNew = targetUser.User;
                QuitUserFromGroup.quitUser(targetUser, _context);

                using (var scope = scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                    group = dbContext.Group.Where(g => g.name == order.groupName).First();

                    Home.Util.GroupNew.launch(sendNew, group, Home.Models.TypeGroupNew.KICK_USER_USER, false, dbContext);
                    Home.Util.GroupNew.launch(sendNew, group, Home.Models.TypeGroupNew.KICK_USER_GROUP, false, dbContext);

                    return Ok(GroupPageManager.GetPage(user, group, dbContext));
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
