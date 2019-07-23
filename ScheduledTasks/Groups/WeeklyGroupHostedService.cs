using API.Areas.Alive.Util;
using API.Data;
using API.ScheduledTasks.Groups.Util;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace API.ScheduledTasks.Groups
{
    public class WeeklyGroupHostedService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory scopeFactory;
        private Timer _timer;

        public WeeklyGroupHostedService(IServiceScopeFactory sf)
        {
            scopeFactory = sf;
        }


        /**
         * Start the service
         */
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                DoWork,
                null,
                CalculateInitalNextTime(), //Next day
                CalculateInitalNextTime() //Next day
            );

            return Task.CompletedTask;
        }


        /**
         * If the application fails, the task and timer will cancel
         */
        public Task StopAsync(CancellationToken cancellationToken)
        {
            //Stop the timer
            _timer.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /**
         * Initialize the database for virtual bets
         */
        private void DoWork(object state)
        {
            try
            { 
                using (var scope = scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                    var hub = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

                    RemoveSessionTokens.remove(dbContext);
                    PayGroups.pay(dbContext, hub);
                    FullyRemoveUsers.remove(dbContext);
                }
            }
            catch (Exception)
            {
                _timer?.Change(TimeSpan.FromDays(1), TimeSpan.FromDays(2));
            }
        }


        /**
         * Delete the timer
         */
        public void Dispose()
        {
            _timer?.Dispose();
        }


        /**
         * Function to get the time next day at 03:05
         * @return TimeSpan
         *      Return the TimeSpan time to the 00:30 the next day
         */
        private TimeSpan CalculateInitalNextTime()
        {
            DateTime nowDay = DateTime.Now;
            nowDay = new DateTime(nowDay.Year, nowDay.Month, nowDay.Day); //Actual day at 00:00

            double now = DateTime.Now
                .ToUniversalTime()
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;

            double then = nowDay.AddDays(1).AddMinutes(30)
                .ToUniversalTime()
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;

            int time = ((int)(then - now)) / 1000;

            int mins = (int)(time / 60); //total min
            int hours = mins / 60; //total hours

            hours = hours % 24; //Exactly hours in one day
            mins = (mins % 60); //Exactly min in one hour

            return new TimeSpan(hours, mins, 0);
        }
    }
}
