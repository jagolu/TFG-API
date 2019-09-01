using API.Areas.Alive.Models;
using API.Areas.Alive.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.GroupManage.Util
{
    /// <summary>
    /// Class to remove a group
    /// </summary>
    public static class RemoveGroup
    {
        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Removes a group
        /// </summary>
        /// <param name="group">The group to remove</param>
        /// <param name="_context">The database context</param>
        /// <param name="hub">The notification hub</param>
        public static void remove(Group group, ApplicationDBContext _context, IHubContext<NotificationHub> hub)
        {
            _context.Entry(group).Collection("users").Load();
            _context.Entry(group).Collection("bets").Load();
            Role maker = RoleManager.getGroupMaker(_context);
            group.bets.ToList().ForEach(bet =>
            {
                _context.Entry(bet).Collection("userBets").Load();
                _context.UserFootballBet.RemoveRange(_context.UserFootballBet.Where(fb => fb.footballBetid == bet.id));
                //_context.Remove(bet);
            });

            _context.UserGroup.Where(ug => ug.groupid == group.id).ToList().ForEach(async us =>
            {
                await sendNews(us, group, _context, hub, maker);
            });

            _context.RemoveRange(group.bets);
            _context.RemoveRange(group.users);
            _context.Remove(group);
            _context.SaveChanges();
        }

        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Send the news and notifications to the members of the group
        /// </summary>
        /// <param name="target">The member of the group to send the new and notification</param>
        /// <param name="group">The group just removed</param>
        /// <param name="_context">The database context</param>
        /// <param name="hub">The notification hub</param>
        /// <param name="roleMaker">The group-maker role</param>
        private static async Task sendNews(UserGroup target, Group group, ApplicationDBContext _context, IHubContext<NotificationHub> hub, Role roleMaker)
        {
            _context.Entry(target).Reference("User").Load();
            _context.Entry(target).Reference("role").Load();

            if (!target.blocked)
            {
                Home.Util.GroupNew.launch(target.User, group, null, Home.Models.TypeGroupNew.REMOVE_GROUP, target.role == roleMaker, _context);
            }
            try
            {
                await SendNotification.send(hub, group.name, target.User, NotificationType.GROUP_REMOVED, _context);
            }
            catch(Exception)
            {
                return;
            }
        }
    }
}
