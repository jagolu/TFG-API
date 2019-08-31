using API.Data;
using API.Data.Models;

namespace API.Util
{
    public static class AdminPolicy
    {
        /// <summary>
        /// Checks if a user is an admin or not
        /// </summary>
        /// <param name="u">The user to check</param>
        /// <param name="context">The database context</param>
        /// <returns>True if the user is an admin, false otherwise</returns>
        public static bool isAdmin(User u, ApplicationDBContext context)
        {
            context.Entry(u).Reference("role").Load();
            Role admin = RoleManager.getAdmin(context);

            return u.role == admin;
        }
    }
}
