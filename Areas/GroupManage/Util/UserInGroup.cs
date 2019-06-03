using API.Data;
using API.Data.Models;
using System;
using System.Linq;

namespace API.Areas.GroupManage.Util
{
    public static class UserInGroup
    {
        public static bool checkUserInGroup(Guid callerId, ref Group group, string groupName, ref UserGroup ugCaller, ApplicationDBContext context)
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
                ugCaller = callerInGroup.First();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
