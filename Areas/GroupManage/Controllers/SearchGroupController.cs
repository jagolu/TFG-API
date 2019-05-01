using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Areas.GroupManage.Models;
using API.Data;
using API.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.GroupManage.Controllers
{
    [Route("Group/[action]")]
    [ApiController]
    public class SearchGroupController : ControllerBase
    {
        private ApplicationDBContext _context;

        public SearchGroupController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("SearchGroup")]
        public List<GroupInfo> checkName(string name)
        {
            //The request name is empty
            if (name == null || name.Length == 0)
            {
                return new List<GroupInfo>();
            }

            List<Group> groupsWithTheSameName = _context.Group.Where(g => g.name.ToLower().Contains(name.ToLower().Trim()) && g.open).ToList();
            List<GroupInfo> groupRet = new List<GroupInfo>();

            groupsWithTheSameName.ForEach(group =>
            {
                _context.Entry(group).Collection("users").Load();
                //_context.Entry(user).Reference("role").Load();

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