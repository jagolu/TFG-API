using API.Data;
using API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.GroupManage.Util
{
    public static class GroupUserManager
    {
        public static bool CheckUserGroup(User caller, ref Group group, string groupName, ref UserGroup targetUserGroup, string publicUserId, ApplicationDBContext context, TypeCheckGroupUser type, bool make_unmake)
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

                group = groups.First();
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
                if(ugCaller.blocked) //The user is blocked
                {
                    return false;
                }
                targetUserGroup = members.Where(m => m.userId != caller.id).First();

                context.Entry(ugCaller).Reference("role").Load();
                context.Entry(targetUserGroup).Reference("role").Load();
                context.Entry(targetUserGroup).Reference("blockedBy").Load();
                string ugCallerRole = ugCaller.role.name;
                string targetUserGroupRole = targetUserGroup.role.name;

                bool can;
                switch (type)
                {
                    case TypeCheckGroupUser.MAKE_ADMIN:
                        can = hasPermissionsMakeAdmin(ugCallerRole, targetUserGroupRole, make_unmake, targetUserGroup.blocked, context);
                        break;
                    case TypeCheckGroupUser.REMOVE_USER:
                        can = hasPermissionsKickUser(ugCallerRole, targetUserGroupRole, targetUserGroup.blocked, targetUserGroup.blocked ? targetUserGroup.blockedBy.name : "", context);
                        break;
                    case TypeCheckGroupUser.BLOCK_USER:
                        can = hasPermissionsBlockUser(ugCallerRole, targetUserGroupRole, targetUserGroup, make_unmake, context);
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

        private static bool hasPermissionsMakeAdmin(string callerRole, string targetRole, bool makeAdmin, bool blocked, ApplicationDBContext context)
        {
            string role_groupMaker = context.Role.Where(r => r.name == "GROUP_MAKER").First().name;
            string nextRole = context.Role.Where(r => r.name == (makeAdmin ? "GROUP_NORMAL" : "GROUP_ADMIN")).First().name;

            if (targetRole != nextRole || callerRole != role_groupMaker || blocked)
            {
                return false;
            }

            return true;
        }

        private static bool hasPermissionsKickUser(string callerRole, string targetRole, bool blocked, string blockedBy, ApplicationDBContext context)
        {
            string role_groupMaker = context.Role.Where(r => r.name == "GROUP_MAKER").First().name;
            string role_groupAdmin = context.Role.Where(r => r.name == "GROUP_ADMIN").First().name;
            string role_normal = context.Role.Where(r => r.name == "GROUP_NORMAL").First().name;

            if(blocked && blockedBy == role_groupMaker && callerRole != role_groupMaker)
            {
                return false;
            }

            if (callerRole != role_groupMaker && (callerRole != role_groupAdmin || targetRole != role_normal))
            {
                return false;
            }

            return true;
        }

        private static bool hasPermissionsBlockUser(string callerRole, string targetRole, UserGroup targetUser, bool make_unmake,ApplicationDBContext context)
        {
            context.Entry(targetUser).Reference("blockedBy").Load();
            bool alreadyBlocked = targetUser.blocked;
            string blockByRole = alreadyBlocked ? targetUser.blockedBy.name : null;
            string role_groupMaker = context.Role.Where(r => r.name == "GROUP_MAKER").First().name;
            string role_normal = context.Role.Where(r => r.name == "GROUP_NORMAL").First().name;

            if(callerRole == role_normal) return false;

            if (!make_unmake)
            {
                if (!alreadyBlocked || (blockByRole==role_groupMaker && callerRole != role_groupMaker)) return false;

                return true;
            }
            else
            {
                if (alreadyBlocked || targetRole == role_groupMaker || callerRole == targetRole)
                {
                    return false;
                }

                return true;
            }
        }
    }

    public enum TypeCheckGroupUser
    {
        MAKE_ADMIN = 1,
        REMOVE_USER = 2,
        BLOCK_USER = 3
    }
}
