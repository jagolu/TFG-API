using API.Data;
using API.Data.Models;
using API.Util;

namespace API.Areas.GroupManage.Util
{
    /// <summary>
    /// Class to check the group-maker functionalities
    /// </summary>
    public static class GroupMakerFuncionlities
    {
        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Check if the caller can do the group-maker funcionality
        /// </summary>
        /// <param name="caller">The user who is gonna do the group-maker funcitonality</param>
        /// <param name="group">A new group object, to save the group on it</param>
        /// <param name="groupName">The name of the group where the user is going to do the action</param>
        /// <param name="type">The id of the group-maker functionality</param>
        /// <param name="_context">The database context</param>
        /// <param name="newPassword">The new password of the group (if is a manage password action)</param>
        /// <param name="oldPassword">The old password of the group (if is a manage password action)</param>
        /// <returns>True if the user can do the action, false otherwise</returns>
        public static bool checkFuncionality(User caller, ref Group group, string groupName, GroupMakerFuncionality type, ApplicationDBContext _context, string newPassword = null, string oldPassword = null)
        {
            UserGroup ugCaller = new UserGroup();

            if (!UserFromGroup.isOnIt(caller.id, ref group, groupName, ref ugCaller, _context))
            {
                return false;
            }

            bool can;
            switch (type)
            {
                case GroupMakerFuncionality.MANAGE_PASSWORD:
                    can = justCheckMaker(ugCaller, _context) && hasPermissionsManagePassword(group, newPassword, oldPassword);
                    break;
                case GroupMakerFuncionality.REMOVE_GROUP:
                    can = justCheckMaker(ugCaller, _context);
                    break;
                case GroupMakerFuncionality.STARTCREATE_FOOTBALL_BET:
                    can = justCheckMaker(ugCaller, _context);
                    break;
                case GroupMakerFuncionality.MANAGEWEEKPAY:
                    can = justCheckMaker(ugCaller, _context);
                    break;
                default:
                    can = false;
                    break;
            }

            return can;
        }


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Check if the caller has permissions to manage the password of the group
        /// </summary>
        /// <param name="group">The group</param>
        /// <param name="newPassword">The new password of the group</param>
        /// <param name="oldPassword">The old password of the group</param>
        /// <returns>True if the user can do the action, false otherwise</returns>
        private static bool hasPermissionsManagePassword(Group group, string newPassword, string oldPassword)
        {
            bool newPass = newPassword != null && newPassword.Length > 0 && PasswordHasher.validPassword(newPassword);
            bool oldPass = oldPassword != null && oldPassword.Length > 0;
            bool hasPassword = group.password != null;

            if ((!oldPass || !hasPassword) && (!newPass || oldPass || hasPassword))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Just check if the caller is the maker of the group
        /// </summary>
        /// <param name="caller">The user who wants to do the action</param>
        /// <param name="dbContext">The database context</param>
        /// <returns>True if the caller is the maker of the group, false otherwise</returns>
        private static bool justCheckMaker(UserGroup caller, ApplicationDBContext dbContext)
        {
            Role groupMaker = RoleManager.getGroupMaker(dbContext);
            dbContext.Entry(caller).Reference("role").Load();

            return caller.role == groupMaker;
        }
    }


    /// <summary>
    /// The ids of the group-maker funcionality
    /// </summary>
    public enum GroupMakerFuncionality
    {
        MANAGE_PASSWORD = 1,
        REMOVE_GROUP = 2,
        STARTCREATE_FOOTBALL_BET = 3,
        MANAGEWEEKPAY = 4
    }
}
