using System.Collections.Generic;
using System.Linq;
using API.Areas.GroupManage.Models;
using API.Areas.Identity.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static API.Areas.GroupManage.Models.GroupInfo;

namespace API.Areas.GroupManage.Controllers
{
    [Route("Group/[action]")]
    [ApiController]
    public class GetAllGroupsControlller : ControllerBase
    {
        private ApplicationDBContext _context;

        public GetAllGroupsControlller(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("GetAllGroups")]
        public List<GroupInfo> getAllGroups()
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!user.open) return new List<GroupInfo>();
            bool isAdmin = AdminPolicy.isAdmin(user, _context);
            List<Group> groups = !isAdmin ? _context.Group.Where(g => g.open).ToList()
                                          : _context.Group.ToList();

            return addGroupsToList(groups, AdminPolicy.isAdmin(user, _context));
        }


        [HttpGet]
        [Authorize]
        [ActionName("SearchGroup")]
        public List<GroupInfo> searchGroupByName(string name)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            name = name.ToLower().Trim();

            if (!user.open) return new List<GroupInfo>();

            //The request name is empty
            if (name == null || name.Length == 0)
            {
                return new List<GroupInfo>();
            }

            bool isAdmin = AdminPolicy.isAdmin(user, _context);
            List<Group> groupsWithTheSameName = !isAdmin ? 
                _context.Group.Where(g => g.name.ToLower().Contains(name) && g.open).ToList() :
                _context.Group.Where(g =>g.name.ToLower().Contains(name)).ToList();

            return addGroupsToList(groupsWithTheSameName, AdminPolicy.isAdmin(user, _context));
        }

        [HttpGet]
        [Authorize]
        [ActionName("ReloadUserGroups")]
        public List<UserGroups> reloadUserGroups()
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!user.open) return new List<UserGroups>();
            List<UserGroups> groupsInfo = new List<UserGroups>();
            _context.Entry(user).Collection("groups").Load();
            
            user.groups.Where(g => !g.blocked).ToList().ForEach(group =>
            {
                _context.Entry(group).Reference("Group").Load();

                groupsInfo.Add(new UserGroups
                {
                    name = group.Group.name,
                    type = group.Group.type
                });
            });

            return groupsInfo;
        }

        private List<GroupInfo> addGroupsToList(List<Group> groups, bool isAdmin)
        {
            List<GroupInfo> groupRet = new List<GroupInfo>();

            groups.ForEach(group =>
            {
                _context.Entry(group).Collection("users").Load();

                groupRet.Add(new GroupInfo
                {
                    name = group.name,
                    type = group.type,
                    open = group.open,
                    password = group.password != null,
                    placesOcupped = group.users.Count(),
                    totalPlaces = group.capacity,
                    dateCreate = group.dateCreated,
                    members = isAdmin ? getGroupMembers(group) : null
                });
            });

            return groupRet;
        }

        private List<GroupMemberAdmin> getGroupMembers(Group group)
        {
            _context.Entry(group).Collection("users").Load();
            List<GroupMemberAdmin> members = new List<GroupMemberAdmin>();

            group.users.ToList().ForEach(u =>
            {
                _context.Entry(u).Reference("User").Load();
                _context.Entry(u).Reference("role").Load();
                members.Add(new GroupMemberAdmin
                {
                    username = u.User.nickname,
                    email = u.User.email,
                    role = u.role.name,
                    dateJoin = u.dateJoin,
                    dateRole = u.dateRole,
                    blocked = u.blocked,
                    coins = u.coins
                });
            });

            return members;
        }
    }
}