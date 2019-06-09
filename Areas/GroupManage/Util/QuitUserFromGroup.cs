using API.Data;
using API.Data.Models;
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
                if (members.Count() == 1) // The user in the group is the only member in
                {
                    _context.Entry(userGroup).Reference("Group").Load();
                    RemoveGroup.Remove(userGroup.Group, _context);
                }
                else
                {
                    Role role_groupMaker = _context.Role.Where(r => r.name == "GROUP_MAKER").First();
                    Role role_groupAdmin = _context.Role.Where(r => r.name == "GROUP_ADMIN").First();
                    Role role_groupNormal = _context.Role.Where(r => r.name == "GROUP_NORMAL").First();

                    //The user is a normal user or an admin in the group, the UserGroup entry is just deleted
                    if (userGroup.role == role_groupMaker)
                    {
                        List<UserGroup> adminMembers = members.Where(m => m.role == role_groupAdmin).OrderBy(d => d.dateRole).ToList();
                        List<UserGroup> normalMembers = members.Where(m => m.role == role_groupNormal).OrderBy(d => d.dateJoin).ToList();
                        UserGroup newMaster;

                        if (adminMembers.Count() != 0) //The older admin in the group will become in the group maker
                        {
                            newMaster = adminMembers.First();
                        }
                        else //If there isn't any admin, the older member in the group will become in the group make
                        {
                            newMaster = normalMembers.First();
                        }

                        newMaster.role = role_groupMaker;
                        newMaster.dateRole = DateTime.Today;

                        _context.Update(newMaster);
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
    }
}
