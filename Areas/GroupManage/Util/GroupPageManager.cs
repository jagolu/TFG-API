using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.Bet.Models;
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
                page.dateJoin = callerInGroup.dateJoin;
                page.dateRole = callerInGroup.dateRole;
                page.actualCapacity = group.users.ToList().Count();
                page.canPutPassword = group.canPutPassword;
                page.createDate = group.dateCreated;
                page.hasPassword = group.password != null;
                page.maxCapacity = group.capacity;
                page.bets = getBets(callerInGroup, group, _context);
                page.members = getMembers(caller.id, callerInGroup_role, group, _context, role_group_normal);

                return page;
            }
            catch (Exception)
            {
                return new GroupPage{
                    name = "",
                    type = false,
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

        private static List<GroupBet> getBets(UserGroup caller, Group group, ApplicationDBContext _context)
        {
            List<GroupBet> bets = new List<GroupBet>();
            _context.Entry(group).Collection("bets").Load();

            group.bets.Where(b => !b.ended && !b.cancelled).ToList().ForEach(bet =>
            {
                _context.Entry(bet).Reference("MatchDay").Load();
                _context.Entry(bet).Collection("userBets").Load();
                _context.Entry(bet.MatchDay).Reference("Competition").Load();
                _context.Entry(bet).Reference("type").Load();
                _context.Entry(bet).Reference("typePay").Load();
                _context.Entry(bet.MatchDay).Reference("HomeTeam").Load();
                _context.Entry(bet.MatchDay).Reference("AwayTeam").Load();
                if(bet.userBets.Where(ub => ub.userId == caller.userId).Count() == 0) bets.Add(new GroupBet
                {
                    bet = bet.id.ToString(),
                    competition = bet.MatchDay.Competition.name,
                    betName = bet.MatchDay.HomeTeam.name +" vs "+bet.MatchDay.AwayTeam.name,
                    typeBet = new NameWinRate
                    {
                        name = bet.type.name,
                        description = bet.type.description,
                        winRate = bet.type.winRate
                    },
                    typePay = new NameWinRate
                    {
                        name = bet.typePay.name,
                        description = bet.typePay.description,
                        winRate = bet.typePay.winRate
                    },
                    minBet = bet.minBet,
                    maxBet = bet.maxBet,
                    matchdayDate = bet.MatchDay.date,
                    lastBetTime = bet.dateLastBet
                });
            });

            return bets;
        }

        private static List<GroupMember> getMembers(Guid callerId, string callerRoleInGroup, Group group, ApplicationDBContext _context, string roleGroup_normal)
        {
            List<GroupMember> members = new List<GroupMember>();
            _context.Entry(group).Collection("users").Load();

            members = addFromList(members, group.users.Where(g => !g.blocked && g.userId != callerId).OrderBy(u => u.dateJoin).ToList(), _context);

            if (callerRoleInGroup != roleGroup_normal)
            {
                members = addFromList(members, group.users.Where(g => g.blocked && g.userId != callerId).ToList(), _context);
            }

            UserGroup ownMember = group.users.Where(g => g.userId == callerId).First();
            members.Add(formatGroupMember(ownMember, _context));

            return members;
        }

        private static List<GroupMember> addFromList(List<GroupMember> mainList, List<UserGroup> outList, ApplicationDBContext _context)
        {
            outList.ForEach(user =>
            {
                mainList.Add(formatGroupMember(user, _context));
            });

            return mainList;
        }


        private static GroupMember formatGroupMember(UserGroup ug, ApplicationDBContext _context)
        {
            _context.Entry(ug).Reference("User").Load();
            _context.Entry(ug).Reference("role").Load();
            _context.Entry(ug).Reference("Group").Load();
            GroupMember ret = new GroupMember
            {
                userName = ug.User.nickname,
                publicUserId = ug.User.publicId,
                role = ug.role.name,
                dateJoin = ug.dateJoin,
                dateRole = ug.dateRole,
                img = ug.User.profileImg,
                blocked = ug.blocked,
                blockedBy = ug.blockedBy != null ? ug.blockedBy.name : ""
            };

            if (!ug.Group.type) ret.coins = ug.coins;

            return ret;
        }
    }
}
