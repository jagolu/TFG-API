using API.Data;
using API.Data.Models;
using System.Linq;

namespace API.Util
{
    public static class AdminPolicy
    {
        public static bool isAdmin(User u, ApplicationDBContext context)
        {
            context.Entry(u).Reference("role").Load();
            Role admin = context.Role.Where(r => r.name == "ADMIN").First();

            return u.role == admin;
        }
    }
}
