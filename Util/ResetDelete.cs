using System;

namespace API.Util
{
    public static class ResetDelete
    {
        /// <summary>
        /// Reset the delete account petition
        /// </summary>
        /// <param name="u">The user who doesn't want to delete the account</param>
        /// <param name="dbContext">The database context</param>
        public static void reset(Data.Models.User u, Data.ApplicationDBContext dbContext)
        {
            try
            {
                u.dateDeleted = null;
                dbContext.SaveChanges();
            }
            catch (Exception) { }
        }
    }
}
