using API.Areas.Alive.Util;
using API.Data;
using API.Data.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;

namespace API.ScheduledTasks.Weekly.Util
{
    public static class PayGroups
    {
        public static void pay(ApplicationDBContext _context, IHubContext<NotificationHub> hub)
        {
            DayOfWeek today = DateTime.Now.DayOfWeek;
            int day = DateTime.Now.DayOfYear;
            int year = DateTime.Now.Year;


            _context.Group.ToList().ForEach(group =>
            {
                if (today != group.dateCreated.DayOfWeek) return;
                if (day == group.dateCreated.DayOfYear && year == group.dateCreated.Year) return;

                _context.Entry(group).Collection("users").Load();
                int moreCoins = group.weeklyPay;
                Areas.Home.Util.GroupNew.launch(null, group, null, Areas.Home.Models.TypeGroupNew.PAID_PLAYERS_GROUPS, false, _context);

                group.users.ToList().ForEach(async u =>
                {
                    u.coins += moreCoins;
                    _context.Entry(u).Reference("User").Load();
                    User recv = u.User;

                    Areas.Home.Util.GroupNew.launch(recv, group, null, Areas.Home.Models.TypeGroupNew.PAID_PLAYERS_USER, false, _context);
                    await SendNotification.send(hub, group.name, recv, Areas.Alive.Models.NotificationType.PAID_GROUPS, _context);
                });
            });

            _context.SaveChanges();
        }
    }
}
