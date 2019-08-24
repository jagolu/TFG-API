using System;

namespace API.ScheduledTasks.VirtualBets.Util
{
    public static class NextScheduledTime
    {
        public static TimeSpan nextTime(int addHours)
        {
            DateTime nowDay = DateTime.Now;
            nowDay = new DateTime(nowDay.Year, nowDay.Month, nowDay.Day); //Actual day at 00:00

            double now = DateTime.Now
                .ToUniversalTime()
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;

            double then = nowDay.AddDays(1).AddHours(addHours).AddMinutes(5)
                .ToUniversalTime()
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;

            int time = ((int)(then - now)) / 1000;

            int mins = (int)(time / 60); //total min
            int hours = mins / 60; //total hours

            hours = hours % 24; //Exactly hours in one day
            mins = mins % 60; //Exactly min in one hour

            return new TimeSpan(hours, mins, 0);
        }
    }
}
