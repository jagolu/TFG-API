using System.Linq;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.GroupManage
{
    [Route("Group/[action]")]
    [ApiController]
    public class CheckGroupNameController : ControllerBase
    {
        private ApplicationDBContext _context;

        public CheckGroupNameController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [EnableCors]
        [ActionName("CheckGroupName")]
        public bool checkName(string name)
        {
            if (name == null)
            {
                return false;
            }
            
            int groupWithTheSameName = _context.Group.Where(g => g.name == name).Count();

            if (groupWithTheSameName != 0)
            {
                return false;
            }

            return true;
        }
    }
}
