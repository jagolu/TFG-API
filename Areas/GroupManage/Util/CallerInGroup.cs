using API.Data;
using API.Data.Models;
using API.Util;
using System.Linq;

namespace API.Areas.GroupManage.Util
{
    public static class CallerInGroup
    {
        public static bool CheckUserCapabilities(User caller, ref Group group, string groupName, TypeCheckCapabilites type, ApplicationDBContext _context, string newPassword = null, string oldPassword = null)
        {
            UserGroup ugCaller = new UserGroup();

            if (!UserInGroup.checkUserInGroup(caller.id, ref group, groupName, ref ugCaller, _context))
            {
                return false;
            }

            bool can;
            switch (type)
            {
                case TypeCheckCapabilites.MANAGE_PASSWORD:
                    can = hasPermissionsManagePassword(ugCaller, group, newPassword, oldPassword, _context);
                    break;
                case TypeCheckCapabilites.REMOVE_GROUP:
                    can = hasPermissionsRemoveGroup(ugCaller, caller, _context);
                    break;
                case TypeCheckCapabilites.STARTCREATE_FOOTBALL_BET:
                    can = hasPermissionsStartCreateFootballBet(ugCaller, group, _context);
                    break;
                default:
                    can = false;
                    break;
            }

            return can;
        }

        private static bool hasPermissionsManagePassword(UserGroup ugCaller, Group group, string newPassword, string oldPassword, ApplicationDBContext _context)
        {
            Role role_groupMaker = RoleManager.getGroupMaker(_context);
            bool role = ugCaller.role == role_groupMaker;
            bool newPass = newPassword != null && newPassword.Length > 0 && PasswordHasher.validPassword(newPassword);
            bool oldPass = oldPassword != null && oldPassword.Length > 0;
            bool canPutPass = group.canPutPassword;
            bool hasPassword = group.password != null;

            if (!role || !canPutPass)
            {
                return false;
            }

            if ((!oldPass || !hasPassword) && (!newPass || oldPass || hasPassword))
            {
                return false;
            }

            return true;
        }

        private static bool hasPermissionsRemoveGroup(UserGroup ugCaller, User caller, ApplicationDBContext _context)
        {
            _context.Entry(ugCaller).Reference("role").Load();
            Role role_groupMaker = RoleManager.getGroupMaker(_context);
            Role caller_role = ugCaller.role;

            if (role_groupMaker != caller_role)
            {
                return false;
            }

            return true;
        }

        private static bool hasPermissionsStartCreateFootballBet(UserGroup ugCaller, Group group, ApplicationDBContext _context)
        {
            _context.Entry(ugCaller).Reference("role").Load();

            Role groupMaker_role = RoleManager.getGroupMaker(_context);
            Role groupAdmin_role = RoleManager.getGroupAdmin(_context);


            if (ugCaller.role != groupMaker_role && ugCaller.role != groupAdmin_role)
            {
                return false;
            }
            if(group.type != false)
            {
                return false;
            }

            return true;
        }
    }






    public enum TypeCheckCapabilites
    {
        MANAGE_PASSWORD = 1,
        REMOVE_GROUP = 2,
        STARTCREATE_FOOTBALL_BET = 3
    }
}
