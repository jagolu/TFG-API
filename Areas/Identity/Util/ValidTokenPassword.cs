﻿using API.Data;
using API.Data.Models;
using System;
using System.Linq;

namespace API.Areas.Identity.Util
{
    public static class ValidTokenPassword
    {
        public static bool isValid(string token, ref User user, ApplicationDBContext dbContext)
        {
            var tokenExists = dbContext.User.Where(u => u.tokenPassword == token);
            if (tokenExists.Count() != 1) //The token doesn't exists
            {
                return false;
            }

            //The token isn't valid
            if (DateTime.Now > tokenExists.First().tokenP_expiresTime)
            {
                return false;
            }
            user = tokenExists.First();

            return true;
        }
    }
}