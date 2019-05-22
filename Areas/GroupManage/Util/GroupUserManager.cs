using API.Data;
using API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.GroupManage.Util
{
    public static class GroupUserManager
    {
        public static bool CheckUserGroup(User caller, /*ref Group group,*/ string groupName, ref UserGroup targetUserGroup, string publicUserId, ApplicationDBContext context, TypeCheckGroupUser type, bool make_unamke)
        {
            try
            {
                var groups = context.Group.Where(g => g.name == groupName); //The group
                var targetUsers = context.User.Where(u => u.publicId == publicUserId); //The user who will be the new admin

                // The group of the target user don't exist
                if (groups.Count() != 1 || targetUsers.Count() != 1)
                {
                    return false;
                }

                Group group = groups.First();
                User targetUser = targetUsers.First();

                context.Entry(group).Collection("users").Load();
                Guid targetUserid = targetUser.id;
                List<UserGroup> members = group.users.Where(u => u.userId == caller.id || u.userId == targetUserid).ToList();

                //The users are not members of the group
                if (members.Count() != 2)
                {
                    return false;
                }

                UserGroup ugCaller = members.Where(m => m.userId == caller.id).First();
                targetUserGroup = members.Where(m => m.userId != caller.id).First();

                context.Entry(ugCaller).Reference("role").Load();
                context.Entry(targetUserGroup).Reference("role").Load();
                string ugCallerRole = ugCaller.role.name;
                string targetUserGroupRole = targetUserGroup.role.name;

                bool can;
                switch (type)
                {
                    case TypeCheckGroupUser.MAKE_ADMIN:
                        can = hasPermissionsMakeAdmin(ugCallerRole, targetUserGroupRole, make_unamke, context);
                        break;
                    case TypeCheckGroupUser.REMOVE_USER:
                        can = hasPermissionsKickUser(ugCallerRole, targetUserGroupRole, context);
                        break;
                    default:
                        can = false;
                        break;
                }

                return can;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool hasPermissionsMakeAdmin(string callerRole, string targetRole, bool makeAdmin, ApplicationDBContext context)
        {
            string role_groupMaker = context.Role.Where(r => r.name == "GROUP_MAKER").First().name;
            string nextRole = context.Role.Where(r => r.name == (makeAdmin ? "GROUP_NORMAL" : "GROUP_ADMIN")).First().name;

            if (targetRole != nextRole || callerRole != role_groupMaker)
            {
                return false;
            }

            return true;
        }

        private static bool hasPermissionsKickUser(string callerRole, string targetRole, ApplicationDBContext context)
        {
            string role_groupMaker = context.Role.Where(r => r.name == "GROUP_MAKER").First().name;
            string role_groupAdmin = context.Role.Where(r => r.name == "GROUP_ADMIN").First().name;
            string role_normal = context.Role.Where(r => r.name == "GROUP_NORMAL").First().name;

            if ((callerRole == role_groupMaker) || (callerRole == role_groupAdmin && targetRole == role_normal))
            {
                return true;
            }

            return false;
        }
    }

    public enum TypeCheckGroupUser
    {
        MAKE_ADMIN = 1,
        REMOVE_USER = 2
    }
}
