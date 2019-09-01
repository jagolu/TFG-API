using API.Data;
using API.Data.Models;
using API.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.GroupManage.Util
{
    /// <summary>
    /// Check the functionalities of a group-admin in a group
    /// </summary>
    public static class GroupAdminFuncionlities
    {
        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //
        
        /// <summary>
        /// Check if a user can do a group-admin functionality
        /// </summary>
        /// <param name="caller">The user who wants to do the funcitonality</param>
        /// <param name="group">A new group object, to save the group on it</param>
        /// <param name="groupName">The name of the group where the user wants to do the action</param>
        /// <param name="ugTarget">A new UserGroup object, to save on it</param>
        /// <param name="publicUserId">The public id of the member of the group who is goins to receive the funcionality</param>
        /// <param name="context">The database context</param>
        /// <param name="type">The id of the group-admin funcitonality</param>
        /// <param name="make_unmake">True to do the funcionality, false to undo it</param>
        /// <returns>True if the user can do the funcionality, false otherwise</returns>
        public static bool checkFuncionality(User caller, ref Group group, string groupName, ref UserGroup ugTarget, string publicUserId, ApplicationDBContext context, GroupAdminFuncionality type, bool make_unmake)
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

                List<User> possibleTargets = context.User.Where(u => u.publicid == publicUserId).ToList(); //The target user

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
                    case GroupAdminFuncionality.MAKE_ADMIN:
                        can = hasPermissionsMakeAdmin(callerRole, targetRole, make_unmake, ugTarget.blocked, context);
                        break;
                    case GroupAdminFuncionality.REMOVE_USER:
                        can = hasPermissionsKickUser(callerRole, targetRole, ugTarget.blocked, ugTarget.blocked ? ugTarget.blockedBy : new Role(), context);
                        break;
                    case GroupAdminFuncionality.BLOCK_USER:
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


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //
        
        /// <summary>
        /// Check if the caller can make/unmake admin to another user
        /// </summary>
        /// <param name="callerRole">The role of the caller in the group</param>
        /// <param name="actualTargetRole">The role of the user in the group who is gonna receive the action</param>
        /// <param name="makeAdmin">True to make admin, false to unmake him</param>
        /// <param name="blocked">True if the user is blocked in the group, false otherwise</param>
        /// <param name="context">The database context</param>
        /// <returns>True if the user can do the action, false otherwise</returns>
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

        /// <summary>
        /// Check if the caller can kick another member of the group
        /// </summary>
        /// <param name="callerRole">The role of the caller in the group</param>
        /// <param name="targetRole">The role of the user in the group who is gonna receive the action</param>
        /// <param name="blocked">True if the user is blocked in the group, false otherwise</param>
        /// <param name="blockedBy">The role of the user that blocked the target user</param>
        /// <param name="context">The database context</param>
        /// <returns>True if the user can do the action, false otherwise</returns>
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

        /// <summary>
        /// Check if the user can block/unblock another user
        /// </summary>
        /// <param name="callerRole">The role of the user who wants to do the action</param>
        /// <param name="targetRole">The role of the target user</param>
        /// <param name="targetUser">The member of the group who is gonna receive the action</param>
        /// <param name="make_unmake">True to block the user, false otherwise</param>
        /// <param name="context">The database context</param>
        /// <returns>True if the user can do the action, false otherwise</returns>
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


    /// <summary>
    /// A id for the group-admin functionality
    /// </summary>
    public enum GroupAdminFuncionality
    {
        MAKE_ADMIN = 1,
        REMOVE_USER = 2,
        BLOCK_USER = 3
    }
}
