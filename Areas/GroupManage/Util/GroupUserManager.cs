using API.Data;
using API.Data.Models;
using API.Util;
using System;
using System.Linq;

namespace API.Areas.GroupManage.Util
{
    public static class GroupUserManager
    {
        public static bool CheckUserGroup(User caller, ref Group group, string groupName, ref UserGroup ugTarget, string publicUserId, ApplicationDBContext context, TypeCheckGroupUser type, bool make_unmake)
        {
            try
            {
                UserGroup ugCaller = new UserGroup();
                //The caller or the group doesnt exist, or the user is not a member of the group
                if (!UserInGroup.checkUserInGroup(caller.id, ref group, groupName, ref ugCaller, context)) 
                {
                    return false;
                }

                if (ugCaller.blocked)
                {
                    return false;
                }

                var targetUsers = context.User.Where(u => u.publicId == publicUserId); //The target user

                if(targetUsers.Count() != 1 || !UserInGroup.checkUserInGroup(targetUsers.First().id, ref group, groupName, ref ugTarget, context, false))
                {
                    return false;
                }

                context.Entry(ugCaller).Reference("role").Load();
                context.Entry(ugTarget).Reference("role").Load();
                context.Entry(ugTarget).Reference("blockedBy").Load();
                string ugCallerRole = ugCaller.role.name;
                string targetUserGroupRole = ugTarget.role.name;

                bool can;
                switch (type)
                {
                    case TypeCheckGroupUser.MAKE_ADMIN:
                        can = hasPermissionsMakeAdmin(ugCallerRole, targetUserGroupRole, make_unmake, ugTarget.blocked, context);
                        break;
                    case TypeCheckGroupUser.REMOVE_USER:
                        can = hasPermissionsKickUser(ugCallerRole, targetUserGroupRole, ugTarget.blocked, ugTarget.blocked ? ugTarget.blockedBy.name : "", context);
                        break;
                    case TypeCheckGroupUser.BLOCK_USER:
                        can = hasPermissionsBlockUser(ugCallerRole, targetUserGroupRole, ugTarget, make_unmake, context);
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
            string role_groupMaker = RoleManager.getGroupMaker(context).name;
            string nextRole = makeAdmin ? RoleManager.getGroupNormal(context).name : RoleManager.getGroupAdmin(context).name;

            if (targetRole != nextRole || callerRole != role_groupMaker || blocked)
            {
                return false;
            }

            return true;
        }

        private static bool hasPermissionsKickUser(string callerRole, string targetRole, bool blocked, string blockedBy, ApplicationDBContext context)
        {
            string role_groupMaker = RoleManager.getGroupMaker(context).name;
            string role_groupAdmin = RoleManager.getGroupAdmin(context).name;
            string role_normal = RoleManager.getGroupNormal(context).name;

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
            string role_groupMaker = RoleManager.getGroupMaker(context).name;
            string role_normal = RoleManager.getGroupNormal(context).name;

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
