using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.Bet.Models;
using API.Areas.GroupManage.Models;
using API.Areas.Home.Models;
using API.Data;
using API.Data.Models;
using API.Util;

namespace API.Areas.GroupManage.Util
{
    /// <summary>
    /// Page to the create the group-page object
    /// </summary>
    public static class GroupPageManager
    {
        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Gets the group page for a member of it
        /// </summary>
        /// <param name="caller">The member of the group who wants the group page</param>
        /// <param name="group">The group</param>
        /// <param name="_context">The database context</param>
        /// <returns>The group page</returns>
        /// See <see cref="Areas.GroupManage.Models.GroupPage"/> to know the response structure
        public static GroupPage GetPage(User caller, Group group, ApplicationDBContext _context)
        {
            try
            {
                _context.Entry(group).Collection("users").Load();
                UserGroup callerInGroup = group.users.Where(u => u.userid == caller.id).First();
                _context.Entry(callerInGroup).Reference("role").Load();

                string callerInGroup_role = callerInGroup.role.name;
                string role_group_normal = RoleManager.getGroupNormal(_context).name;
                string role_group_maker = RoleManager.getGroupMaker(_context).name;
                string role_group_admin = RoleManager.getGroupAdmin(_context).name;

                GroupPage page = new GroupPage();
                page.name = group.name;
                page.dateJoin = callerInGroup.dateJoin;
                page.dateRole = callerInGroup.dateRole;
                page.actualCapacity = group.users.ToList().Count();
                page.createDate = group.dateCreated;
                page.hasPassword = group.password != null;
                page.weeklyPay = group.weeklyPay;
                page.maxCapacity = group.capacity;
                page.bets = getBets(caller, group, _context);
                page.manageBets = getManageBets(caller, group, _context);
                page.myBets = getBetAndUserBets(caller, group, _context, false);
                page.betsHistory = getBetAndUserBets(caller, group, _context, true);
                page.members = getMembers(caller.id, callerInGroup_role, group, _context, role_group_normal);
                page.news = Home.Util.GetNews.getGroupNews(group.id, _context);

                return page;
            }
            catch (Exception)
            {
                return new GroupPage {
                    name = "",
                    bets = new List<GroupBet>(),
                    members = new List<GroupMember>(),
                    actualCapacity = 0,
                    createDate = new DateTime(),
                    hasPassword = false,
                    maxCapacity = 0,
                    weeklyPay = 0,
                    dateJoin = new DateTime(),
                    dateRole = new DateTime(),
                    manageBets = new List<BetsManager>(),
                    myBets = new List<EndedFootballBet>(),
                    betsHistory = new List<EndedFootballBet>(),
                    news = new List<NewMessage>()
                };
            }
        }


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Get the new bets of this week for the group
        /// </summary>
        /// <param name="caller">The member of the group who wants the group page</param>
        /// <param name="group">The group</param>
        /// <param name="_context">The database context</param>
        /// <returns>The new fb of the groups</returns>
        /// See <see cref="Areas.Bet.Models.GroupBet"/> to know the response strucure
        private static List<GroupBet> getBets(User caller, Group group, ApplicationDBContext _context)
        {
            List<GroupBet> bets = new List<GroupBet>();
            _context.Entry(group).Collection("bets").Load();

            group.bets.Where(b => !b.ended && !b.cancelled && b.dateEnded>DateTime.Now && b.dateLastBet>DateTime.Now).OrderByDescending(bb=> bb.dateReleased).ToList().ForEach(bet =>
            {
                _context.Entry(bet).Collection("userBets").Load();
                if (bet.userBets.Where(ub => ub.userid == caller.id && ub.valid).Count() == 0)
                {
                    bets.Add(new GroupBet(bet, _context, false));
                }
            });

            return bets;
        }

        /// <summary>
        /// Get the active fb and the fb history
        /// </summary>
        /// <param name="caller">The member of the group who wants the group page</param>
        /// <param name="group">The group</param>
        /// <param name="_context">The database context</param>
        /// <param name="ended">True to get the fb history, false to get the active fb</param>
        /// <returns>A list of ended fb</returns>
        /// See <see cref="Areas.Bet.Models.EndedFootballBet"/> to know the response structure
        private static List<EndedFootballBet> getBetAndUserBets(User caller, Group group, ApplicationDBContext _context, bool ended)
        {
            List<EndedFootballBet> history = new List<EndedFootballBet>();
            _context.Entry(group).Collection("bets").Load();
            _context.Entry(group).Collection("users").Load();

            group.bets.Where(b=> b.ended==ended).OrderByDescending(bb => bb.dateReleased).ToList().ForEach(bet =>
            {
                _context.Entry(bet).Collection("userBets").Load();
                if (bet.userBets.Where(b => b.userid == caller.id).Count() > 0)
                {
                    history.Add(new EndedFootballBet(caller, bet, _context, ended));
                }
            });
            return history;
        }

        /// <summary>
        /// Get the members of the group
        /// </summary>
        /// <param name="callerId">The id of the caller member of the group</param>
        /// <param name="callerRoleInGroup">The name of the role of the caller member of the group</param>
        /// <param name="group">The group</param>
        /// <param name="_context">The database context</param>
        /// <param name="roleGroup_normal">The name of the normal role in the group</param>
        /// <returns></returns>
        private static List<GroupMember> getMembers(Guid callerId, string callerRoleInGroup, Group group, ApplicationDBContext _context, string roleGroup_normal)
        {
            List<GroupMember> members = new List<GroupMember>();
            _context.Entry(group).Collection("users").Load();

            members = addFromList(members, group.users.Where(g => !g.blocked && g.userid != callerId).OrderBy(u => u.dateJoin).ToList(), _context);

            if (callerRoleInGroup != roleGroup_normal)
            {
                members = addFromList(members, group.users.Where(g => g.blocked && g.userid != callerId).ToList(), _context);
            }

            UserGroup ownMember = group.users.Where(g => g.userid == callerId).First();
            members.Add(formatGroupMember(ownMember, _context));

            return members;
        }

        /// <summary>
        /// Add a member to the list of members
        /// </summary>
        /// <param name="mainList">The main list with the til now users</param>
        /// <param name="outList">The list of the members of the group</param>
        /// <param name="_context">The database context</param>
        /// <returns>The list of the members of the group</returns>
        private static List<GroupMember> addFromList(List<GroupMember> mainList, List<UserGroup> outList, ApplicationDBContext _context)
        {
            outList.ForEach(user =>
            {
                mainList.Add(formatGroupMember(user, _context));
            });

            return mainList;
        }

        /// <summary>
        /// Transform a UserGroup object to a GroupMember object
        /// </summary>
        /// <param name="ug">The UserGroup object</param>
        /// <param name="_context">The database context</param>
        /// <returns>The group member object</returns>
        private static GroupMember formatGroupMember(UserGroup ug, ApplicationDBContext _context)
        {
            _context.Entry(ug).Reference("User").Load();
            _context.Entry(ug).Reference("role").Load();
            _context.Entry(ug).Reference("Group").Load();
            GroupMember ret = new GroupMember
            {
                userName = ug.User.nickname,
                publicUserId = ug.User.publicid,
                role = ug.role.name,
                dateJoin = ug.dateJoin,
                dateRole = ug.dateRole,
                img = ug.User.profileImg,
                blocked = ug.blocked,
                blockedBy = ug.blockedBy != null ? ug.blockedBy.name : "",
                coins = ug.coins
            };

            return ret;
        }

        /// <summary>
        /// Get the manage fb objects for the group maker
        /// </summary>
        /// <param name="caller">The caller member of the group</param>
        /// <param name="group">The group</param>
        /// <param name="_context">The database context</param>
        /// <returns>The list of fb that maker can manage</returns>
        private static List<BetsManager> getManageBets(User caller, Group group, ApplicationDBContext _context)
        {
            List<BetsManager> bets = new List<BetsManager>();
            _context.Entry(group).Collection("bets").Load();
            _context.Entry(group).Collection("users").Load();
            _context.Entry(caller).Reference("role").Load();
            Role maker = RoleManager.getGroupMaker(_context);

            if (maker != group.users.Where(u=>u.userid == caller.id).First().role)
            {
                return null;
            }

            group.bets.OrderByDescending(o => o.dateReleased).ToList().ForEach(b =>
            {
                bets.Add(new BetsManager
                {
                    bet = new GroupBet(b, _context, b.ended || b.cancelled),
                    dateLaunch = b.dateReleased,
                    ended = b.ended,
                    dateEnd = b.dateEnded,
                    dateCancelled = b.dateCancelled,
                    cancelled = b.cancelled,
                    betId = b.id.ToString(),
                    canBeCancelled = b.dateEnded > DateTime.Now
                });
            });

            return bets;
        }
    }
}
