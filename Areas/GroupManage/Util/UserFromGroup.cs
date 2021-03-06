﻿using API.Data;
using API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.GroupManage.Util
{
    /// <summary>
    /// Class to check if a user belongs to a group
    /// </summary>
    public static class UserFromGroup
    {
        /// <summary>
        /// Check if user belongs to a group
        /// </summary>
        /// <param name="callerId">The id of the user</param>
        /// <param name="group">A new group object, to save the group on it</param>
        /// <param name="groupName">The name of the group</param>
        /// <param name="ugCaller">A new UserGroup object, to save the group member on it</param>
        /// <param name="context">The database context</param>
        /// <param name="matterBlock">True if for the functionality matters if the member is blocked, false if it doesn't</param>
        /// <returns>True if the user belongs to the group, false otherwise</returns>
        public static bool isOnIt(Guid callerId, ref Group group, string groupName, ref UserGroup ugCaller, ApplicationDBContext context, bool matterBlock = true)
        {
            try
            {
                List<Group> possibleGroups = context.Group.Where(g => g.name == groupName).ToList(); //The group
                if(possibleGroups.Count() != 1)
                {
                    return false;
                }

                List<UserGroup> callerInGroup = context.UserGroup.Where(ug => ug.groupid == possibleGroups.First().id && ug.userid == callerId).ToList();
                if(callerInGroup.Count() != 1)
                {
                    return false;
                }
                if(matterBlock && callerInGroup.First().blocked)
                {
                    return false;
                }

                ugCaller = callerInGroup.First(); //The member on the group
                group = possibleGroups.First();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
