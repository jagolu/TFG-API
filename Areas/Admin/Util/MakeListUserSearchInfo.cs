using API.Areas.Admin.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using System.Collections.Generic;
using System.Linq;
using static API.Areas.Admin.Models.UserSearchInfo;

namespace API.Areas.Admin.Util
{
    public static class MakeListUserSearchInfo
    {
        public static List<UserSearchInfo> make(List<User> users, ApplicationDBContext dbContext)
        {
            List<UserSearchInfo> usersRet = new List<UserSearchInfo>();

            users.ForEach(user =>
            {
                dbContext.Entry(user).Collection("groups").Load();
                List<UserInGroup> uGroups = new List<UserInGroup>();
                Role admin = RoleManager.getAdmin(dbContext);

                dbContext.UserGroup.Where(ug => ug.userid == user.id && ug.Group.open).ToList().ForEach(g =>
                {
                    dbContext.Entry(g).Reference("Group").Load();
                    dbContext.Entry(g).Reference("role").Load();
                    uGroups.Add(new UserInGroup
                    {
                        groupName = g.Group.name,
                        role = g.role.name,
                        blocked = g.blocked,
                        joinTime = g.dateJoin,
                        roleTime = g.dateRole
                    });
                });

                usersRet.Add(new UserSearchInfo
                {
                    publicUserId = user.publicid,
                    email = user.email,
                    username = user.nickname,
                    open = user.open,
                    dateSignUp = user.dateSignUp,
                    groups = uGroups
                });
            });

            return usersRet;
        }
    }
}
