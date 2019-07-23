using System;

namespace API.Util
{
    public static class ResetDelete
    {
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
