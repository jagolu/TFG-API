using API.Data;
using API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.ScheduledTasks.Weekly.Util
{
    public static class RemoveSessionTokens
    {
        public static void remove(ApplicationDBContext _context)
        {
            DateTime now = DateTime.Now;
            List<UserToken> delete = _context.UserToken.Where(t => t.expirationTime < now).ToList();

            _context.UserToken.RemoveRange(delete);
            _context.SaveChanges();
        }
    }
}
