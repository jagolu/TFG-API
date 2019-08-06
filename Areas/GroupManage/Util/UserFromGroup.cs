using API.Data;
using API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.GroupManage.Util
{
    public static class UserFromGroup
    {
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
