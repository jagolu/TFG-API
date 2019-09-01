using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.GroupManage.Controllers
{
    [Route("Group/[action]")]
    [ApiController]
    public class CreateGroupController : ControllerBase
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
        public CreateGroupController(ApplicationDBContext context)
        {
            _context = context;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        [HttpGet]
        [Authorize]
        [ActionName("CreateGroup")]
        /// <summary>
        /// Create a new group
        /// </summary>
        /// <param name="groupName">The name of the new group</param>
        /// <returns>The IActionResult of the create group action</returns>
        public IActionResult createGroup([Required][MaxLength(30)][MinLength(4)] string groupName )
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");

            //Group with the same name
            var dbGroup = _context.Group.Where(g => g.name == groupName);

            if (dbGroup.Count() > 0) //If already exists a group with the same name
            {
                return BadRequest();
            }

            if (!canCreateANewGroup(user))
            {
                return BadRequest(new { error = "LimitationCreateGroup" });
            }

            if (!canCreateAGroup(user)) //The user cant create more groups right now
            {
                return BadRequest(new { error = "LimitationTimeCreateGroup" });
            }

            try
            {
                Group newGroup = new Group { name = groupName };
                
                UserGroup userG = new UserGroup{
                    User = user,
                    Group = newGroup,
                    role = RoleManager.getGroupMaker(_context),
                    dateRole = DateTime.Today,
                    coins = newGroup.weeklyPay
                };

                user.lastTimeCreateGroup = DateTime.Now;

                _context.Add(newGroup);
                _context.Add(userG);

                _context.SaveChanges();

                Home.Util.GroupNew.launch(user, newGroup, null, Home.Models.TypeGroupNew.CREATE_GROUP_GROUP, false, _context);
                Home.Util.GroupNew.launch(user, newGroup, null, Home.Models.TypeGroupNew.CREATE_GROUP_USER, false, _context);

                return Ok(new { success = "SuccesfullCreatedGroup" });
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
        /// Check if the user can create a new group
        /// </summary>
        /// <param name="user">The user who wants to create a new group</param>
        /// <returns>True if the user can create a new group, false otherwise</returns>
        private bool canCreateAGroup(User user)
        {
            if (user.lastTimeCreateGroup == null)
            {
                return true;
            }
            if (user.lastTimeCreateGroup.Value.AddDays(7) > DateTime.Now)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if the user can create a new group ignorign its type
        /// </summary>
        /// <param name="user">The user who tries to create the new group</param>
        /// <returns>True if the user can create another group, false otherwhise</returns>
        private bool canCreateANewGroup(User user)
        {
            int totalUserGroupJoined = _context.UserGroup.Where(ug => ug.userid == user.id ).Count();

            return totalUserGroupJoined < user.maxGroupJoins;
        }
    }
}
