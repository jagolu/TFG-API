using API.Areas.Identity.Models;
using API.Areas.UserInfo.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using System;
using System.Collections.Generic;

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
                    List<string> groups = GroupsOfUser.get(user, context);
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
    }
}
