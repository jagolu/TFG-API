using API.Data;
using API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.ScheduledTasks.Weekly.Util
{
    /// <summary>
    /// Remove a user completely (only his User data)
    /// </summary>
    public static class FullyRemoveUsers
    {
        /// <summary>
        /// Remove a user
        /// </summary>
        /// <param name="dbContext">The database context</param>
        public static void remove(ApplicationDBContext dbContext)
        {
            DateTime lastWeek = DateTime.Now.AddDays(-7);
            List<User> delete = dbContext.User.Where(u => u.dateDeleted != null && u.dateDeleted<lastWeek).ToList();

            dbContext.RemoveRange(delete);
            dbContext.SaveChanges();
        }
    }
}
