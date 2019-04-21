using System.Linq;
using API.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.GroupManage.Controllers
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

        /**
         * Check if a group name is available
         * @param name The new group name
         * @return True if the new group name is available, false otherwise
         */
        [HttpGet]
        [ActionName("CheckGroupName")]
        public bool checkName(string name)
        {
            //The request name is empty
            if (name == null || name.Length == 0)
            {
                return false;
            }

            int groupWithTheSameName = _context.Group.Where(g => g.name == name).Count();

            // Already exists a group with the requested name
            if (groupWithTheSameName != 0)
            {
                return false;
            }
                
            return true;
        }
    }
}
