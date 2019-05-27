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
    public class ManagePasswordController : ControllerBase
    {
        private ApplicationDBContext _context;

        public ManagePasswordController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("ManagePassword")]
        public IActionResult managePassword([FromBody] ManagePassword order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to make admin to another user
            UserGroup ugCaller = new UserGroup();
            Group group = new Group();

            if(!UserInGroup.checkUserInGroup(user.id, ref group, order.name, ref ugCaller, _context))
            {
                return BadRequest();
            }
            if(!hasPermissions(ugCaller, group, order.newPassword, order.oldPassword))
            {
                return BadRequest();
            }
            if(group.password!=null && !PasswordHasher.areEquals(order.oldPassword, group.password))
            {
                return BadRequest(new { error = "IncorrectOldPassword" });
            }

            group.password = order.newPassword == null ? null : PasswordHasher.hashPassword(order.newPassword);
            _context.Update(group);
            _context.SaveChanges();
            
            return Ok(GroupPageManager.GetPage(user, group, _context));
        }

        private bool hasPermissions(UserGroup ugCaller, Group group, string newPassword, string oldPassword)
        {
            Role role_groupMaker = _context.Role.Where(r => r.name == "GROUP_MAKER").First();
            bool role = ugCaller.role == role_groupMaker;
            bool newPass = newPassword != null && newPassword.Length > 0 && PasswordHasher.validPassword(newPassword);
            bool oldPass = oldPassword != null && oldPassword.Length > 0;
            bool canPutPass = group.canPutPassword;
            bool hasPassword = group.password != null;

            if(!role || !canPutPass)
            {
                return false;
            }

            if (  (!oldPass || !hasPassword) && (!newPass || oldPass || hasPassword) )
            {
                return false;
            }

            return true;
        }
    }
}
