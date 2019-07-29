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
        private ApplicationDBContext _context;

        public CreateGroupController(ApplicationDBContext context)
        {
            _context = context;
        }


        /**
         * Create a new group
         * @param group The new group to create
         * @return 500 The group already exists
         * @return 401 LimitationCreateGroup The user can't create more groups of the specificated type
         * @return 200 The group has been created sucesfully
         */ 
        [HttpGet]
        [Authorize]
        [ActionName("CreateGroup")]
        public IActionResult createGroup([Required][MaxLength(20)][MinLength(4)] string groupName )
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

            if(!canCreateAnewSpecificGroup(user)) //The user cant create more groups
            {
                return BadRequest(new { error = "LimitationSpecificCreateGroup" });
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

        /**
         * Check if an user can create a new specific group
         * @param {bool} type The type of the new group to create
         * @user The user trying to create the new group
         * @return true if the user can create the new group, false otherwise
         */
        private bool canCreateAnewSpecificGroup(User user)
        {
            //int userGroups = 0;
            //int limitationGroups = 0;

            //userGroups = _context.UserGroup.Where(ug =>
            //    ug.userId == user.id &&
            //    ug.role.name == "GROUP_MAKER").Count();

            //limitationGroups = user.createGroup;

            //return userGroups < limitationGroups; //The user cant create a new group of the specificated type
            return true;
        }

        /**
         * Function to know if the user can create a new group ignoring its type
         * 
         * @access private
         * @param {User} The user who tries to create the new group
         * @return {bool} True if the user can create another group, false otherwhise
         */
        private bool canCreateANewGroup(User user)
        {
            int totalUserGroupJoined = _context.UserGroup.Where(ug => ug.userId == user.id ).Count();

            return totalUserGroupJoined < user.maxGroupJoins;
        }
    }
}
