using API.Areas.Identity.Models;
using API.Data;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Util
{
    public class GroupsFromUser
    {
        public static List<UserGroups> getUserGroups(User u, ApplicationDBContext _context)
        {
            List<UserGroups> userGroups = new List<UserGroups>();
            List<UserGroup> groups = _context.UserGroup.Where(ug => ug.userId == u.id).ToList();

            groups.ForEach(g =>
            {
                _context.Entry(g).Reference("Group").Load();
                _context.Entry(g).Reference("role").Load();

                userGroups.Add(new UserGroups
                {
                    name = g.Group.name,
                    type = g.Group.type
                });
            });

            return userGroups;
        }
    }
}
