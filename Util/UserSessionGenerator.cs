﻿using API.Areas.Identity.Models;
using API.Data;
using API.Models;
using System;

namespace API.Util
{
    public static class UserSessionGenerator
    {
        public static UserSession getUserJson(ApplicationDBContext context, User user, Boolean provider)
        {
            string nToken = TokenGenerator.generateTokenAndRefreshToken(context, user.email, provider);

            context.Entry(user).Reference("role").Load();

            UserSession session = new UserSession {
                api_token = nToken,
                role = user.role.name
            };

            return session;
        }
    }
}
