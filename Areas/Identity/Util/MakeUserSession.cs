using API.Areas.Identity.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.Identity.Util
{
    public static class MakeUserSession
    {

        public static UserSession getUserSession(ApplicationDBContext context, User user, Boolean provider)
        {
            try
            {
                UserSession session = getUserJson(context, user, provider);

                if(session != null)
                {
                    List<UserGroups> groups = getUserGroups(user, context);
                    session.groups = groups;
                    context.SaveChanges();
                    return session;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static UserSession getUserJson(ApplicationDBContext context, User user, Boolean provider)
        {
            string nToken = TokenGenerator.generateTokenAndRefreshToken(context, user.email, provider);

            context.Entry(user).Reference("role").Load();

            UserSession session = new UserSession
            {
                api_token = nToken,
                role = user.role.name,
                username = user.nickname
            };

            return session;
        }

        private static List<UserGroups> getUserGroups(User u, ApplicationDBContext _context)
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
                    type = g.Group.type,
                    role = g.role.name
                });
            });

            return userGroups;
        }
    }
}
