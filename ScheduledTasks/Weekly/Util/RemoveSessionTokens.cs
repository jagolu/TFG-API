using API.Data;
using API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.ScheduledTasks.Weekly.Util
{
    /// <summary>
    /// Class to manage the refresh session tokens of the users
    /// </summary>
    public static class RemoveSessionTokens
    {
        /// <summary>
        /// Remove the expired refresh session tokens of the users
        /// </summary>
        /// <param name="_context">The database context</param>
        public static void remove(ApplicationDBContext _context)
        {
            DateTime now = DateTime.Now;
            List<UserToken> delete = _context.UserToken.Where(t => t.expirationTime < now).ToList();

            _context.UserToken.RemoveRange(delete);
            _context.SaveChanges();
        }
    }
}
