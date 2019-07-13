using API.Data;
using API.Data.Models;

namespace API.Util
{
    public static class AdminPolicy
    {
        public static bool isAdmin(User u, ApplicationDBContext context)
        {
            context.Entry(u).Reference("role").Load();
            Role admin = RoleManager.getAdmin(context);

            return u.role == admin;
        }
    }
}
