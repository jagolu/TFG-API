using API.Areas.Alive.Util;
using API.Data;
using API.ScheduledTasks.VirtualBets.Util;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.ScheduledTasks.VirtualBets
{
    public class PayFootballBetHostedService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory scopeFactory;
        private Timer _timer;

        public PayFootballBetHostedService(IServiceScopeFactory sf)
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
               TimeSpan.Zero,
               //CalculateInitalNextTime(), //30 minutes from now, to wait the football database is initialized (Free azure background services are so slow -.-)
               CalculateInitalNextTime() //tomorrow
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
                    List<Data.Models.Group> groupsNews = new List<Data.Models.Group>();

                    dbContext.FootballBets.Where(fb => !fb.ended && !fb.cancelled).ToList().ForEach(bet =>
                    {
                        dbContext.Entry(bet).Reference("MatchDay").Load();
                        if (bet.MatchDay.status == "FINISHED")
                        {
                            CheckWinners.checkWinner(bet, dbContext, hub);
                            bet.ended = true;
                            dbContext.SaveChanges();

                            //Add the group to launch the pay-new
                            dbContext.Entry(bet).Reference("Group").Load();
                            if (!groupsNews.Any(g => g.id == bet.groupid)) groupsNews.Add(bet.Group);
                        }
                    });
                }

                //Set cron next day
                _timer?.Change(CalculateInitalNextTime(), CalculateInitalNextTime());
            }
            catch (Exception)
            {
                _timer?.Change(TimeSpan.FromHours(1), TimeSpan.FromHours(2));
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
         *      Return the TimeSpan time to the 03:05 the next day
         */
        private TimeSpan CalculateInitalNextTime()
        {
            DateTime nowDay = DateTime.Now;
            nowDay = new DateTime(nowDay.Year, nowDay.Month, nowDay.Day); //Actual day at 00:00

            double now = DateTime.Now
                .ToUniversalTime()
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;

            double then = nowDay.AddDays(1).AddHours(3)
                .ToUniversalTime()
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;

            int time = ((int)(then - now)) / 1000;

            int mins = (int)(time / 60); //total min
            int hours = mins / 60; //total hours

            hours = hours % 24; //Exactly hours in one day
            mins = (mins % 60) + 5; //Exactly min in one hour

            return new TimeSpan(hours, mins, 0);
        }
    }
}
