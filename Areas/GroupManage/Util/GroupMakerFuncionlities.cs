using API.Data;
using API.Data.Models;
using API.Util;

namespace API.Areas.GroupManage.Util
{
    public static class GroupMakerFuncionlities
    {
        public static bool checkFuncionlity(User caller, ref Group group, string groupName, GroupMakerFuncionlity type, ApplicationDBContext _context, string newPassword = null, string oldPassword = null)
        {
            UserGroup ugCaller = new UserGroup();

            if (!UserFromGroup.isOnIt(caller.id, ref group, groupName, ref ugCaller, _context))
            {
                return false;
            }

            bool can;
            switch (type)
            {
                case GroupMakerFuncionlity.MANAGE_PASSWORD:
                    can = justCheckMaker(ugCaller, _context) && hasPermissionsManagePassword(group, newPassword, oldPassword);
                    break;
                case GroupMakerFuncionlity.REMOVE_GROUP:
                    can = justCheckMaker(ugCaller, _context);
                    break;
                case GroupMakerFuncionlity.STARTCREATE_FOOTBALL_BET:
                    can = justCheckMaker(ugCaller, _context);
                    break;
                case GroupMakerFuncionlity.MANAGEWEEKPAY:
                    can = justCheckMaker(ugCaller, _context);
                    break;
                default:
                    can = false;
                    break;
            }

            return can;
        }

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

        private static bool justCheckMaker(UserGroup caller, ApplicationDBContext dbContext)
        {
            Role groupMaker = RoleManager.getGroupMaker(dbContext);

            return isMaker(caller, groupMaker, dbContext);
        }

        private static bool isMaker(UserGroup ugCaller, Role maker, ApplicationDBContext _context)
        {
            _context.Entry(ugCaller).Reference("role").Load();

            return ugCaller.role == maker;
        }
    }






    public enum GroupMakerFuncionlity
    {
        MANAGE_PASSWORD = 1,
        REMOVE_GROUP = 2,
        STARTCREATE_FOOTBALL_BET = 3,
        MANAGEWEEKPAY = 4
    }
}
