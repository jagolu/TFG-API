using System.Linq;
using API.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.GroupManage.Controllers
{
    [Route("Group/[action]")]
    [ApiController]
    public class CheckGroupNameController : ControllerBase
    {
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The database context of the application</value>
        private ApplicationDBContext _context;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The database context</param>
        public CheckGroupNameController(ApplicationDBContext context)
        {
            _context = context;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        [HttpGet]
        [ActionName("CheckGroupName")]
        /// <summary>
        /// Check if a group name is available
        /// </summary>
        /// <param name="name">The new group name</param>
        /// <returns>True if the new group name is available, false otherwise</returns>
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
