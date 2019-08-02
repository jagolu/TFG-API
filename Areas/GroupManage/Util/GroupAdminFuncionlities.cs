using API.Data;
using API.Data.Models;
using API.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.GroupManage.Util
{
    public static class GroupAdminFuncionlities
    {
        public static bool checkFuncionality(User caller, ref Group group, string groupName, ref UserGroup ugTarget, string publicUserId, ApplicationDBContext context, GroupAdminFuncionlity type, bool make_unmake)
        {
            try
            {
                UserGroup ugCaller = new UserGroup();
                //The caller or the group doesnt exist, or the user is not a member of the group
                if (!UserFromGroup.isOnIt(caller.id, ref group, groupName, ref ugCaller, context)) 
                {
                    return false;
                }
                if (ugCaller.blocked)
                {
                    return false;
                }

                List<User> possibleTargets = context.User.Where(u => u.publicId == publicUserId).ToList(); //The target user

                if(possibleTargets.Count() != 1 || !UserFromGroup.isOnIt(possibleTargets.First().id, ref group, groupName, ref ugTarget, context, false))
                {
                    return false;
                }

                context.Entry(ugCaller).Reference("role").Load();
                context.Entry(ugTarget).Reference("role").Load();
                context.Entry(ugTarget).Reference("blockedBy").Load();
                Role callerRole = ugCaller.role;
                Role targetRole = ugTarget.role;

                bool can;
                switch (type)
                {
                    case GroupAdminFuncionlity.MAKE_ADMIN:
                        can = hasPermissionsMakeAdmin(callerRole, targetRole, make_unmake, ugTarget.blocked, context);
                        break;
                    case GroupAdminFuncionlity.REMOVE_USER:
                        can = hasPermissionsKickUser(callerRole, targetRole, ugTarget.blocked, ugTarget.blocked ? ugTarget.blockedBy : new Role(), context);
                        break;
                    case GroupAdminFuncionlity.BLOCK_USER:
                        can = hasPermissionsBlockUser(callerRole, targetRole, ugTarget, make_unmake, context);
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

        private static bool hasPermissionsMakeAdmin(Role callerRole, Role actualTargetRole, bool makeAdmin, bool blocked, ApplicationDBContext context)
        {
            Role groupMaker = RoleManager.getGroupMaker(context); 
            Role nextRole = makeAdmin ? RoleManager.getGroupAdmin(context) : RoleManager.getGroupNormal(context);

            if (actualTargetRole == nextRole || callerRole != groupMaker || blocked)
            {
                return false;
            }

            return true;
        }

        private static bool hasPermissionsKickUser(Role callerRole, Role targetRole, bool blocked, Role blockedBy, ApplicationDBContext context)
        {
            Role groupMaker = RoleManager.getGroupMaker(context);
            Role groupAdmin = RoleManager.getGroupAdmin(context);
            Role groupNormal = RoleManager.getGroupNormal(context);

            if(blocked && blockedBy == groupMaker && callerRole != groupMaker)
            {
                return false;
            }

            if (callerRole != groupMaker && (callerRole != groupAdmin || targetRole != groupNormal))
            {
                return false;
            }

            return true;
        }

        private static bool hasPermissionsBlockUser(Role callerRole, Role targetRole, UserGroup targetUser, bool make_unmake, ApplicationDBContext context)
        {
            context.Entry(targetUser).Reference("blockedBy").Load();
            bool alreadyBlocked = targetUser.blocked;
            Role blockBy = alreadyBlocked ? targetUser.blockedBy : new Role();
            Role groupMaker = RoleManager.getGroupMaker(context);
            Role groupNormal = RoleManager.getGroupNormal(context);

            if(callerRole == groupNormal) return false;

            if ((!make_unmake) && (!alreadyBlocked || (blockBy==groupMaker && callerRole != groupMaker)))
            {
                return false;
            }
            else if (make_unmake && (alreadyBlocked || targetRole == groupMaker || callerRole == targetRole))
            {
                return false;
            }

            return true;
        }
    }

    public enum GroupAdminFuncionlity
    {
        MAKE_ADMIN = 1,
        REMOVE_USER = 2,
        BLOCK_USER = 3
    }
}
