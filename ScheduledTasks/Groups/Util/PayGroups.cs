using API.Data;
using System;
using System.Linq;

namespace API.ScheduledTasks.Groups.Util
{
    public static class PayGroups
    {
        public static void pay(ApplicationDBContext _context)
        {
            DayOfWeek today = DateTime.Now.DayOfWeek;
            int day = DateTime.Now.DayOfYear;
            int year = DateTime.Now.Year;

            _context.Group.Where(g => 
                                        g.dateCreated.DayOfWeek == today && 
                                        g.dateCreated.DayOfYear != day && 
                                        g.dateCreated.Year != year
            ).ToList().ForEach(group =>
            {
                _context.Entry(group).Collection("users").Load();
                int moreCoins = group.weeklyPay;

                group.users.ToList().ForEach(u =>
                {
                    u.coins += moreCoins;
                });
            });

            _context.SaveChanges();
        }
    }
}
