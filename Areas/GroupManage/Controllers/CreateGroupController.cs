using System;
using System.Linq;
using API.Areas.GroupManage.Models;
using API.Data;
using API.Models;
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
        [HttpPost]
        [Authorize]
        [ActionName("CreateGroup")]
        public IActionResult createGroup([FromBody] CreateGroup group )
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);

            //Group with the same name
            var dbGroup = _context.Group.Where(g => g.name == group.name);

            if (dbGroup.Count() > 0) //If already exists a group with the same name
            {
                return StatusCode(500);
            }

            if(!canCreateANewGroup(group, user)) //The user cant create more groups
            {
                return BadRequest(new { error = "LimitationCreateGroup" });
            }

            try
            {
                Group newGroup = new Group { name = group.name, type = group.type };
                
                UserGroup userG = new UserGroup{
                    User = user,
                    Group = newGroup,
                    role = _context.Role.Where(r => r.name == "GROUP_MAKER").First(),
                    dateRole = DateTime.Today
                };

                _context.Add(newGroup);
                _context.Add(userG);

                _context.SaveChanges();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            return Ok();
        }

        /**
         * Check if an user can create a new specific group
         * @param group New group to create
         * @user The user trying to create the new group
         * @return true if the user can create the new group, false otherwise
         */
        private bool canCreateANewGroup(CreateGroup group, User user)
        {
            int userGroups = 0;
            int limitationGroups = 0;
            _context.Entry(user).Reference("limitations").Load();
            _context.Entry(user).Reference("role").Load();

            if (group.type) //Official group
            {
                //Official groups created by the user
                userGroups = _context.UserGroup.Where(ug =>
                    ug.userId == user.id &&
                    ug.Group.type == true &&
                    ug.role.name == "GROUP_MAKER").Count();

                //Max groups that the user can create
                limitationGroups = user.limitations.createOfficialGroup;
            }
            else //Virtual group
            {
                //Virtual groups created by the user
                userGroups = _context.UserGroup.Where(ug =>
                    ug.userId == user.id &&
                    ug.Group.type == false &&
                    ug.role.name == "GROUP_MAKER").Count();

                //Max groups that the user can create
                limitationGroups = user.limitations.createVirtualGroup;
            }

            if (limitationGroups <= userGroups) //The user cant create a new group of the specificated type
            {
                return false;
            }

            return true;
        }
    }
}
