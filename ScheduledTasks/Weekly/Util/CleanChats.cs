using API.Data;
using API.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace API.ScheduledTasks.Weekly.Util
{
    /// <summary>
    /// Class to clean the chat history
    /// </summary>
    public static class CleanChats
    {
        /// <summary>
        /// Leaves every chat with 100 messages or less
        /// </summary>
        /// <param name="dbContext">The context of the database</param>
        public static void clean(ApplicationDBContext dbContext)
        {
            dbContext.Group.ToList().ForEach(g =>
            {
                List<GroupChatMessage> msgs = dbContext.GroupChatMessage.Where(c => c.groupid == g.id).ToList();

                int outOfRange = 100 - msgs.Count();

                if (outOfRange < 0)
                {
                    outOfRange *= -1;
                    msgs.TakeLast(outOfRange);
                    dbContext.RemoveRange(msgs.TakeLast(outOfRange));
                    dbContext.SaveChanges();
                }
            });
        }
    }
}
