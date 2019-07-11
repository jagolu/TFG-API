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
            Group group = new Group();

            if(!CallerInGroup.CheckUserCapabilities(user, ref group, order.name, TypeCheckCapabilites.MANAGE_PASSWORD, _context, order.newPassword, order.oldPassword))
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

            Home.Util.GroupNew.launch(null, group, Home.Models.TypeGroupNew.MAKE_PRIVATE, group.password != null, _context);
            
            return Ok(GroupPageManager.GetPage(user, group, _context));
        }
    }
}
