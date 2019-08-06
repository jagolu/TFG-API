using API.Areas.UserInfo.Models;
using API.Data;
using API.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.UserInfo.Util
{
    public static class UserRoleGroups
    {
        public static List<RoleGroup> get(User user, ApplicationDBContext _context)
        {
            List<RoleGroup> roleGroups = new List<RoleGroup>();
            _context.Entry(user).Collection("groups").Load();

            user.groups.ToList().ForEach(
                group => {
                    _context.Entry(group).Reference("Group").Load();
                    _context.Entry(group).Reference("role").Load();

                    roleGroups.Add(new RoleGroup
                    {
                        name = group.Group.name,
                        role = group.role.name
                    });
                }
            );

            return roleGroups;
        }
    }
}
