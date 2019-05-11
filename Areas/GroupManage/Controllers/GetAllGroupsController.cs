using System.Collections.Generic;
using System.Linq;
using API.Areas.GroupManage.Models;
using API.Data;
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

            _context.Group.ToList().ForEach(group =>
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