using System.Collections.Generic;
using System.Linq;
using API.Areas.GroupManage.Models;
using API.Areas.Identity.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            List<GroupInfo> groupRet = new List<GroupInfo>();

            return addGroupsToList(_context.Group.ToList());
        }


        [HttpGet]
        [Authorize]
        [ActionName("SearchGroup")]
        public List<GroupInfo> searchGroupByName(string name)
        {
            //The request name is empty
            if (name == null || name.Length == 0)
            {
                return new List<GroupInfo>();
            }

            List<Group> groupsWithTheSameName = _context.Group.Where(g => 
                g.name.ToLower().Contains(
                    name.ToLower().Trim()
                ) && g.open
            ).ToList();

            return addGroupsToList(groupsWithTheSameName);
        }

        [HttpGet]
        [Authorize]
        [ActionName("ReloadUserGroups")]
        public List<UserGroups> reloadUserGroups()
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            List<UserGroups> groupsInfo = new List<UserGroups>();
            _context.Entry(user).Collection("groups").Load();
            
            user.groups.ToList().ForEach(group =>
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

        private List<GroupInfo> addGroupsToList(List<Group> groups)
        {
            List<GroupInfo> groupRet = new List<GroupInfo>();

            groups.ForEach(group =>
            {
                _context.Entry(group).Collection("users").Load();

                groupRet.Add(new GroupInfo
                {
                    name = group.name,
                    type = group.type,
                    password = group.password != null,
                    placesOcupped = group.users.Count(),
                    totalPlaces = group.capacity
                });
            });

            return groupRet;
        }
    }
}