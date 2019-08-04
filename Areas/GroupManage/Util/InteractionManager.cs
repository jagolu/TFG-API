using API.Data;
using API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.GroupManage.Util
{
    public static class InteractionManager
    {
        public static void manageInteraction(User user, Group group, interactionType type, ApplicationDBContext dbContext)
        {
            List<GroupInteraction> interactions = dbContext.GroupInteractions.Where(gi => gi.groupId == group.id && gi.userId == user.id).ToList();
            bool leftTheGroup = type == interactionType.LEAVED ? true : false;
            bool wasKickedFromTheGroup = type == interactionType.KICKED ? true : false;

            try
            {
                if (interactions.Count() != 1)
                {
                    GroupInteraction gi = new GroupInteraction
                    {
                        Group = group,
                        User = user,
                        kicked = wasKickedFromTheGroup,
                        leaved = leftTheGroup
                    };
                    dbContext.Add(gi);
                }
                else
                {
                    GroupInteraction gi = interactions.First();
                    gi.dateLeave = DateTime.Now;
                    gi.kicked = wasKickedFromTheGroup;
                    gi.leaved = leftTheGroup;
                }
                dbContext.SaveChanges();
            }
            catch (Exception) { }
        }
    }

    public enum interactionType
    {
        NONE = 0,
        KICKED = 1,
        LEAVED = 2,
    }
}
