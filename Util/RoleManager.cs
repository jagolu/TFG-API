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
    }
}
