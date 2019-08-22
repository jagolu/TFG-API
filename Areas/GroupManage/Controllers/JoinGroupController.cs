using System;
using System.Collections.Generic;
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
    public class JoinGroupController : ControllerBase
    {
        private ApplicationDBContext _context;

        public JoinGroupController(ApplicationDBContext context)
        {
            _context = context;
        }


        [HttpPost]
        [Authorize]
        [ActionName("JoinGroup")]
        public IActionResult joinGroup([FromBody] JoinGroup order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
            Group group = new Group();

            if (!isUnderLimitations(user))
            {
                return BadRequest(new { error = "MaxGroupJoinsReached" });
            }
            if(!hasPermissions(user, ref group, order.groupName))
            {
                return BadRequest();
            }

            if(group.password != null && !PasswordHasher.areEquals(order.password, group.password))
            {
                return BadRequest(new { error = "IncorrectPasswordJoiningGroup" });
            }
            if (!group.open)
            {
                return BadRequest(new { error = "GroupBanned" });
            }
            interactionType type = checkInteractions(user, group);
            if(type != interactionType.NONE)
            {
                if(type == interactionType.KICKED)
                {
                    return BadRequest(new { error = "YouwereKickedGroup" });
                }
                else
                {
                    return BadRequest(new { error = "YouhasleavedGroup" });
                }
            }

            try
            {
                UserGroup newUser = new UserGroup
                {
                    User = user,
                    Group = group,
                    role = RoleManager.getGroupNormal(_context),
                    dateRole = DateTime.Today,
                    coins = group.weeklyPay
                };

                _context.UserGroup.Add(newUser);
                _context.SaveChanges();
                Home.Util.GroupNew.launch(user, group, null, Home.Models.TypeGroupNew.JOIN_LEFT_GROUP, true, _context);
                Home.Util.GroupNew.launch(user, group, null, Home.Models.TypeGroupNew.JOIN_LEFT_USER, true, _context);

                return Ok(new { success="SuccesfullJoinGroup"});
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        private bool hasPermissions(User user, ref Group group, string groupName)
        {
            var dbGroup = _context.Group.Where(g => g.name == groupName);

            if (dbGroup.Count() != 1)
            {
                return false;
            }
            group = dbGroup.First();
            _context.Entry(group).Collection("users").Load();

            if (_context.UserGroup.Where(ug => ug.userid == user.id && ug.groupid == dbGroup.First().id).Count() != 0)
            {
                return false;
            }
            if(group.users.Count() >= group.capacity)
            {
                return false;
            }

            return true;
        }

        private bool isUnderLimitations(User user)
        {
            _context.Entry(user).Collection("groups").Load();

            if (user.groups.Where(g=> !g.blocked).ToList().Count() >= user.maxGroupJoins)
            {
                return false;
            }
            return true;
        }

        private interactionType checkInteractions(User user, Group group)
        {
            List<GroupInteraction> interactions = _context.GroupInteractions.Where(ginteraction => ginteraction.userid == user.id && ginteraction.groupid == group.id).ToList();

            if(interactions.Count() == 0)
            {
                return interactionType.NONE;
            }

            GroupInteraction gi = interactions.First();
            if(gi.dateLeave.AddDays(7) < DateTime.Now)
            {
                return interactionType.NONE;
            }

            if (gi.kicked)
            {
                return interactionType.KICKED;
            }
            else
            {
                return interactionType.LEAVED;
            }
        }
    }
}
