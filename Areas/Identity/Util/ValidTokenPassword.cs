using API.Data;
using API.Data.Models;
using System;
using System.Linq;

namespace API.Areas.Identity.Util
{
    public static class ValidTokenPassword
    {
        /// <summary>
        /// Checks if a password token is valid
        /// </summary>
        /// <param name="token">The password token to validate</param>
        /// <param name="user">A new user object, to save the user owner of the token on it</param>
        /// <param name="dbContext">The database context</param>
        /// <returns>True if the token is valid, false otherwise</returns>
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
