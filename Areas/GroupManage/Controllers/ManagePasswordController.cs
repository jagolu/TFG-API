using API.Areas.GroupManage.Models;
using API.Areas.GroupManage.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace API.Areas.GroupManage.Controllers
{
    [Route("Group/[action]")]
    [ApiController]
    public class ManagePasswordController : ControllerBase
    {
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The database context of the application</value>
        private ApplicationDBContext _context;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The database context</param>
        public ManagePasswordController(ApplicationDBContext context)
        {
            _context = context;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        [HttpPost]
        [Authorize]
        [ActionName("ManagePassword")]
        /// <summary>
        /// Mange the password of the group
        /// </summary>
        /// <param name="order">The info to manage the password of the group</param>
        /// See <see cref="Areas.GroupManage.Models.ManagePassword"/> to know the param structure
        /// <returns>The updated group page</returns>
        /// See <see cref="Areas.GroupManage.Models.GroupPage"/> to know the response structure
        public IActionResult managePassword([FromBody] ManagePassword order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to make admin to another user
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
            Group group = new Group();

            if(!GroupMakerFuncionlities.checkFuncionality(user, ref group, order.name, GroupMakerFuncionality.MANAGE_PASSWORD, _context, order.newPassword, order.oldPassword))
            {
                return BadRequest();
            }
            if(group.password!=null && !PasswordHasher.areEquals(order.oldPassword, group.password))
            {
                return BadRequest(new { error = "IncorrectOldPassword" });
            }
            if (!group.open) return BadRequest(new { error = "GroupBanned" });

            try
            {
                group.password = order.newPassword == null ? null : PasswordHasher.hashPassword(order.newPassword);
                _context.Update(group);
                _context.SaveChanges();

                Home.Util.GroupNew.launch(null, group, null, Home.Models.TypeGroupNew.MAKE_PRIVATE, group.password != null, _context);

                return Ok(GroupPageManager.GetPage(user, group, _context));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
