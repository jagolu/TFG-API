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
                _context.Entry(group).Collection("users").Load();
                UserGroup callerInGroup = group.users.Where(u => u.userId == caller.id).First();
                _context.Entry(callerInGroup).Reference("role").Load();

                string callerInGroup_role = callerInGroup.role.name;
                string role_group_normal = _context.Role.Where(r => r.name == "GROUP_NORMAL").First().name;
                string role_group_maker = _context.Role.Where(r => r.name == "GROUP_MAKER").First().name;
                string role_group_admin = _context.Role.Where(r => r.name == "GROUP_ADMIN").First().name;

                GroupPage page = new GroupPage();
                page.name = group.name;
                page.type = group.type;
                page.role = callerInGroup_role;
                page.actualCapacity = group.users.ToList().Count();
                page.canPutPassword = group.canPutPassword;
                page.createDate = group.dateCreated;
                page.hasPassword = group.password != null;
                page.maxCapacity = group.capacity;
                page.bets = getBets(_context);
                page.members = getMembers(caller.id, callerInGroup_role, group, _context, role_group_normal);

                return page;
            }
            catch (Exception e)
            {
                return new GroupPage{
                    name = "",
                    type = false,
                    role = "",
                    bets = new List<GroupBet>(),
                    members = new List<GroupMember>(),
                    actualCapacity = 0,
                    canPutPassword = false,
                    createDate = new DateTime(),
                    hasPassword = false,
                    maxCapacity = 0
                };
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

        private static List<GroupMember> getMembers(Guid callerId, string callerRoleInGroup, Group group, ApplicationDBContext _context, string roleGroup_normal)
        {
            List<GroupMember> members = new List<GroupMember>();
            _context.Entry(group).Collection("users").Load();

            group.users.ToList().ForEach(user =>
            {
                if( callerId != user.userId && (!user.blocked || (user.blocked && callerRoleInGroup != roleGroup_normal)))
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
