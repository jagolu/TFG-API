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
        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Make the usersession object for a user
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="user">The user who wants the session </param>
        /// <param name="provider">The provider of the caller</param>
        /// <returns>The session of the user</returns>
        /// See <see cref="Areas.Identity.Models.UserSession"/> to know the response structure
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


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Get the user session without the groups
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="user">The user who wants the session object</param>
        /// <param name="provider">The provider of the caller</param>
        /// <returns>The session without the groups</returns>
        /// See <see cref="Areas.Identity.Models.UserSession"/> to know the response structure
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
