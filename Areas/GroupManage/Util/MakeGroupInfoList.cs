using API.Areas.GroupManage.Models;
using API.Data;
using API.Data.Models;
using System.Collections.Generic;
using System.Linq;
using static API.Areas.GroupManage.Models.GroupInfo;

namespace API.Areas.GroupManage.Util
{
    public static class MakeGroupInfoList
    {
        public static List<GroupInfo> make(List<Group> groups, bool isAdmin, ApplicationDBContext dbContext)
        {
            List<GroupInfo> groupRet = new List<GroupInfo>();

            groups.ForEach(group =>
            {
                dbContext.Entry(group).Collection("users").Load();

                groupRet.Add(new GroupInfo
                {
                    name = group.name,
                    open = group.open,
                    password = group.password != null,
                    placesOcupped = group.users.Count(),
                    totalPlaces = group.capacity,
                    dateCreate = group.dateCreated,
                    members = isAdmin ? getGroupMembers(group, dbContext) : null
                });
            });

            return groupRet;
        }


        private static List<GroupMemberAdmin> getGroupMembers(Group group, ApplicationDBContext dbContext)
        {
            dbContext.Entry(group).Collection("users").Load();
            List<GroupMemberAdmin> members = new List<GroupMemberAdmin>();

            group.users.ToList().ForEach(u =>
            {
                dbContext.Entry(u).Reference("User").Load();
                dbContext.Entry(u).Reference("role").Load();
                members.Add(new GroupMemberAdmin
                {
                    username = u.User.nickname,
                    email = u.User.email,
                    role = u.role.name,
                    dateJoin = u.dateJoin,
                    dateRole = u.dateRole,
                    blocked = u.blocked,
                    coins = u.coins
                });
            });

            return members;
        }
    }
}
