using API.Data;
using API.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.UserInfo.Util
{
    public static class GroupsOfUser
    {
        /// <summary>
        /// Get the groups of a user
        /// </summary>
        /// <param name="user">The user who we want to know the groups</param>
        /// <param name="_context">The database context</param>
        /// <returns>A list with the groups of the user</returns>
        public static List<string> get(User user, ApplicationDBContext _context)
        {
            List<string> userGroups = new List<string>();
            List<UserGroup> groups = _context.UserGroup.Where(ug => ug.userid == user.id && !ug.blocked).ToList();

            groups.ForEach(g =>
            {
                _context.Entry(g).Reference("Group").Load();
                if (g.Group.open)
                {
                    userGroups.Add(g.Group.name);
                }
            });

            return userGroups;
        }
    }
}
