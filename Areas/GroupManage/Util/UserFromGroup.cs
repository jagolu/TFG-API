using API.Data;
using API.Data.Models;
using System;
using System.Linq;

namespace API.Areas.GroupManage.Util
{
    public static class UserFromGroup
    {
        public static bool isOnIt(Guid callerId, ref Group group, string groupName, ref UserGroup ugCaller, ApplicationDBContext context, bool matterBlock = true)
        {
            try
            {
                var groups = context.Group.Where(g => g.name == groupName); //The group

                if(groups.Count() != 1)
                {
                    return false;
                }
                group = groups.First();

                var callerInGroup = context.UserGroup.Where(ug => ug.groupId == groups.First().id && ug.userId == callerId);
                if(callerInGroup.Count() != 1)
                {
                    return false;
                }
                if(matterBlock && callerInGroup.First().blocked)
                {
                    return false;
                }
                ugCaller = callerInGroup.First(); //The member on the group

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
