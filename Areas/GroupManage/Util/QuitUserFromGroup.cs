using API.Data;
using API.Data.Models;
using API.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.GroupManage.Util
{
    public static class QuitUserFromGroup
    {
        public static bool quitUser(UserGroup userGroup, ApplicationDBContext _context)
        {
            List<UserGroup> members = _context.UserGroup.Where(ug => ug.groupId == userGroup.groupId && !ug.blocked).ToList();

            try
            {
                removeBets(userGroup, _context);

                if (members.Count() == 1) // The user in the group is the only member in
                {
                    _context.Entry(userGroup).Reference("Group").Load();
                    RemoveGroup.Remove(userGroup.Group, _context);
                }
                else
                {
                    Role role_groupMaker = RoleManager.getGroupMaker(_context);
                    Role role_groupAdmin = RoleManager.getGroupAdmin(_context);
                    Role role_groupNormal = _context.Role.Where(r => r.name == "GROUP_NORMAL").First();

                    //The user is a normal user or an admin in the group, the UserGroup entry is just deleted
                    if (userGroup.role == role_groupMaker)
                    {
                        manageQuitMaker(members, role_groupMaker, role_groupAdmin, role_groupNormal, _context);
                    }

                    _context.Remove(userGroup);
                    _context.SaveChanges();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void manageQuitMaker(List<UserGroup> members, Role maker, Role admin, Role normal, ApplicationDBContext _context)
        {
            List<UserGroup> adminMembers = members.Where(m => m.role == admin).OrderBy(d => d.dateRole).ToList();
            List<UserGroup> normalMembers = members.Where(m => m.role == normal).OrderBy(d => d.dateJoin).ToList();
            UserGroup newMaster;

            if (adminMembers.Count() != 0) //The older admin in the group will become in the group maker
            {
                newMaster = adminMembers.First();
            }
            else //If there isn't any admin, the older member in the group will become in the group make
            {
                newMaster = normalMembers.First();
            }

            newMaster.role = maker;
            newMaster.dateRole = DateTime.Today;

            _context.Update(newMaster);
            _context.SaveChanges();
        }

        private static void removeBets(UserGroup ug, ApplicationDBContext context)
        {
            context.Entry(ug).Reference("User").Load();
            context.Entry(ug.User).Collection("footballBets").Load();
            ug.User.footballBets.ToList().ForEach(b =>
            { 
                context.Remove(b);
            });

            context.SaveChanges();
        }
    }
}
