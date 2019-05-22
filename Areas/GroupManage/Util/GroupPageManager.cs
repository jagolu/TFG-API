using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.GroupManage.Models;
using API.Data;
using API.Data.Models;

namespace API.Areas.GroupManage.Util
{
    public static class GroupPageManager
    {
        public static GroupPage GetPage(User caller, Group group, ApplicationDBContext _context)
        {
            try
            {
                UserGroup callerInGroup = _context.UserGroup.Where(ug => ug.userId == caller.id).First();
                _context.Entry(callerInGroup).Reference("role").Load();

                string callerInGroup_role = callerInGroup.role.name;
                string role_group_normal = _context.Role.Where(r => r.name == "GROUP_NORMAL").First().name;
                string role_group_maker = _context.Role.Where(r => r.name == "GROUP_MAKER").First().name;
                string role_group_admin = _context.Role.Where(r => r.name == "GROUP_ADMIN").First().name;

                GroupPage page = new GroupPage();
                page.groupName = group.name;
                page.groupType = group.type;
                page.role = callerInGroup_role;
                page.bets = getBets(_context);
                page.members = getMembers(callerInGroup_role, group, _context, role_group_normal);

                return page;
            }
            catch (Exception)
            {
                return new GroupPage();
            }

        }

        private static List<GroupBet> getBets(ApplicationDBContext _context)
        {
            return new List<GroupBet>{
                new GroupBet { betName = "bet1", betBody = "betBody1"},
                new GroupBet { betName = "bet2", betBody = "betBody2"},
                new GroupBet { betName = "bet3", betBody = "betBody3"},
                new GroupBet { betName = "bet4", betBody = "betBody4"},
                new GroupBet { betName = "bet5", betBody = "betBody5"}
            };
        }

        private static List<GroupMember> getMembers(string callerRoleInGroup, Group group, ApplicationDBContext _context, string roleGroup_normal)
        {
            List<GroupMember> members = new List<GroupMember>();
            _context.Entry(group).Collection("users").Load();

            group.users.ToList().ForEach(user =>
            {
                if(!user.blocked || (user.blocked && callerRoleInGroup != roleGroup_normal))
                {
                    _context.Entry(user).Reference("blockedBy").Load();
                    _context.Entry(user).Reference("User").Load();

                    members.Add(new GroupMember
                    {
                        userName = user.User.nickname,
                        publicUserId = user.User.publicId,
                        role = user.role.name,
                        dateJoin = user.dateJoin,
                        dateRole = user.dateRole,
                        img = user.User.profileImg,
                        blocked = user.blocked,
                        blockedBy = user.blockedBy != null ? user.blockedBy.name : ""
                    });
                }
            });

            return members;
        }
    }
}
