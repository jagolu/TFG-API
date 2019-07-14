using API.Data;
using System;
using System.Linq;

namespace API.ScheduledTasks.Groups.Util
{
    public static class RemoveSessionTokens
    {
        public static void remove(ApplicationDBContext _context)
        {
            _context.UserToken.RemoveRange(
                _context.UserToken.Where(t => t.expirationTime < DateTime.Now).ToList()
            );

            _context.SaveChanges();
        }
    }
}
