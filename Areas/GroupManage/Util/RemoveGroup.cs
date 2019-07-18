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
    public static class RemoveGroup
    {
        public static void Remove(Group group, ApplicationDBContext _context, IHubContext<NotificationHub> hub)
        {
            _context.Entry(group).Collection("users").Load();
            _context.Entry(group).Collection("bets").Load();
            Role maker = RoleManager.getGroupMaker(_context);
            group.bets.ToList().ForEach(bet =>
            {
                _context.Entry(bet).Collection("userBets").Load();
                _context.UserFootballBet.RemoveRange(_context.UserFootballBet.Where(fb => fb.FootballBetId == bet.id));
                //_context.Remove(bet);
            });

            _context.UserGroup.Where(ug => ug.groupId == group.id).ToList().ForEach(async us =>
            {
                await sendNews(us, group, _context, hub, maker);
            });

            _context.RemoveRange(group.bets);
            _context.RemoveRange(group.users);
            _context.Remove(group);
            _context.SaveChanges();
        }


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
