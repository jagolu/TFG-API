using API.Data;
using API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.GroupManage.Util
{
    /// <summary>
    /// Class to manage the interactions of a user with a group
    /// </summary>
    public static class InteractionManager
    {
        /// <summary>
        /// Manages the interaction of a user with a group
        /// </summary>
        /// <param name="user">The user who has interacted with the group</param>
        /// <param name="group">The group</param>
        /// <param name="type">The interaction id</param>
        /// <param name="dbContext">The database context</param>
        public static void manageInteraction(User user, Group group, interactionType type, ApplicationDBContext dbContext)
        {
            List<GroupInteraction> interactions = dbContext.GroupInteractions.Where(gi => gi.groupid == group.id && gi.userid == user.id).ToList();
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


    /// <summary>
    /// The id of the interaction of a user with a group
    /// </summary>
    public enum interactionType
    {
        NONE = 0,
        KICKED = 1,
        LEAVED = 2,
    }
}
