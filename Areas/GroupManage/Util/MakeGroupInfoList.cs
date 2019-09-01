using API.Areas.GroupManage.Models;
using API.Data;
using API.Data.Models;
using System.Collections.Generic;
using System.Linq;
using static API.Areas.GroupManage.Models.GroupInfo;

namespace API.Areas.GroupManage.Util
{
    /// <summary>
    /// Makes a group info list
    /// </summary>
    public static class MakeGroupInfoList
    {
        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Get a list of group and their members
        /// </summary>
        /// <param name="groups">The groups to get the info</param>
        /// <param name="isAdmin">True if the caller is an admin</param>
        /// <param name="dbContext">The database context</param>
        /// <returns>A list of group and their members</returns>
        /// See <see cref="Areas.GroupManage.Models.GroupInfo"/> to know the response structure
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


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Get a list of the members of the group
        /// </summary>
        /// <param name="group">The group</param>
        /// <param name="dbContext">The database context</param>
        /// <returns>A list of the members of the group</returns>
        /// See <see cref="Areas.GroupManage.Models.GroupInfo.GroupMemberAdmin"/> to know the response structure
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
