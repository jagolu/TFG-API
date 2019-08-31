using API.Data;
using API.Data.Models;
using System.Linq;

namespace API.Util
{
    public static class RoleManager
    {
        /// <summary>
        /// Gets the group maker role
        /// </summary>
        /// <param name="_context">The database contexdt</param>
        /// <returns>The group maker role</returns>
        public static Role getGroupMaker(ApplicationDBContext _context)
        {
            return _context.Role.Where(r => r.name == "GROUP_MAKER").First();
        }

        /// <summary>
        /// Gets the group admin role
        /// </summary>
        /// <param name="_context">The database contexdt</param>
        /// <returns>The group admin role</returns>
        public static Role getGroupAdmin(ApplicationDBContext _context)
        {
            return _context.Role.Where(r => r.name == "GROUP_ADMIN").First();
        }

        /// <summary>
        /// Gets the group normal role
        /// </summary>
        /// <param name="_context">The database contexdt</param>
        /// <returns>The group normal role</returns>
        public static Role getGroupNormal(ApplicationDBContext _context)
        {
            return _context.Role.Where(r => r.name == "GROUP_NORMAL").First();
        }

        /// <summary>
        /// Gets the admin role
        /// </summary>
        /// <param name="_context">The database contexdt</param>
        /// <returns>The admin role</returns>
        public static Role getAdmin(ApplicationDBContext _context)
        {
            return _context.Role.Where(r => r.name == "ADMIN").First();
        }

        /// <summary>
        /// Gets the normal role
        /// </summary>
        /// <param name="_context">The database contexdt</param>
        /// <returns>The normal role</returns>
        public static Role getNormalUser(ApplicationDBContext _context)
        {
            return _context.Role.Where(r => r.name == "NORMAL_USER").First();
        }
    }
}
