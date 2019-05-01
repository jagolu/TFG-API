using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Areas.GroupManage.Models;
using API.Data;
using API.Data.Models;
using Microsoft.AspNetCore.Http;
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
        [ActionName("SearchGroup")]
        public List<GroupNameAndType> checkName(string name)
        {
            //The request name is empty
            if (name == null || name.Length == 0)
            {
                return new List<GroupNameAndType>();
            }

            List<Group> groupsWithTheSameName = _context.Group.Where(g => g.name.ToLower().Contains(name.ToLower().Trim()) && g.open).ToList();
            List<GroupNameAndType> groupRet = new List<GroupNameAndType>();

            groupsWithTheSameName.ForEach(group =>
            {
                groupRet.Add(new GroupNameAndType
                {
                    name = group.name,
                    type = group.type
                });
            });

            return groupRet;
        }
    }
}