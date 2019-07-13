using API.Data;
using API.Data.Models;
using System.Linq;

namespace API.Util
{
    public static class RoleManager
    {
        public static Role getGroupMaker(ApplicationDBContext _context)
        {
            return _context.Role.Where(r => r.name == "GROUP_MAKER").First();
        }

        public static Role getGroupAdmin(ApplicationDBContext _context)
        {
            return _context.Role.Where(r => r.name == "GROUP_ADMIN").First();
        }

        public static Role getGroupNormal(ApplicationDBContext _context)
        {
            return _context.Role.Where(r => r.name == "GROUP_NORMAL").First();
        }

        public static Role getAdmin(ApplicationDBContext _context)
        {
            return _context.Role.Where(r => r.name == "ADMIN").First();
        }

        public static Role getNormalUser(ApplicationDBContext _context)
        {
            return _context.Role.Where(r => r.name == "NORMAL_USER").First();
        }
    }
}
