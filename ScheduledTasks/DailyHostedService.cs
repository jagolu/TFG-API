using API.Areas.Alive.Util;
using API.Data;
using API.ScheduledTasks.VirtualBets.Util;
using API.ScheduledTasks.Weekly.Util;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace API.ScheduledTasks
{
    public class DailyHostedService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;
        private IConfiguration _configuration;
        private readonly IHttpClientFactory _http;

        public DailyHostedService(IServiceScopeFactory sf, IConfiguration config, IHttpClientFactory http)
        {
            _scopeFactory = sf;
            _configuration = config;
            _http = http;
        }

        /**
         * Start the service
         */
        public Task StartAsync(CancellationToken cancellationToken)
        {
            nextTime(0);
            _timer = new Timer(
                DoWork,
                null,
                nextTime(0), 
                new TimeSpan(1, 0, 0, 0) //The next day
            );

            return Task.CompletedTask;
        }

        /**
         * If the application fails, the task and timer will cancel
         */
        public Task StopAsync(CancellationToken cancellationToken)
        {
            //Restart the timer
            _timer?.Change(nextTime(0), new TimeSpan(1, 0, 0, 0));

            return Task.CompletedTask;
        }

        /**
         * Initialize the database for virtual bets
         */
        private async void DoWork(object state)
        {
            //TODO initialize the database
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    //Get the database context and the notification context
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();


                    //Initialize & update football data
                    InitializerVirtualDB initializer = new InitializerVirtualDB(dbContext, _configuration, _http);

                    await initializer.InitializeAsync("PL");
                    await initializer.InitializeAsync("PD");
                    await initializer.InitializeAsync("BL1");
                    await initializer.InitializeAsync("SA");
                    await initializer.InitializeAsync("FL1");
                }
                catch (Exception)
                {
                    //Restart the timer
                    _timer?.Change(nextTime(0), new TimeSpan(1, 0, 0, 0));
                }
            }

            //TODO initialize the database
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    //Get the database context and the notification context
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                    var hub = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

                    // Pay the bets
                    dbContext.FootballBets.Where(fb => !fb.ended && !fb.cancelled).ToList().ForEach(bet =>
                    {
                        dbContext.Entry(bet).Reference("MatchDay").Load();
                        if (bet.MatchDay.status == "FINISHED")
                        {
                            CheckWinners.checkWinner(bet, dbContext, hub);
                            bet.ended = true;
                            dbContext.SaveChanges();
                        }
                    });
                }
                catch (Exception)
                {
                    //Restart the timer
                    _timer?.Change(nextTime(0), new TimeSpan(1, 0, 0, 0));
                }
            }

            //TODO initialize the database
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    //Get the database context and the notification context
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                    var hub = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

                    //Remove session tokens
                    RemoveSessionTokens.remove(dbContext);

                    //Pay weekly coins
                    PayGroups.pay(dbContext, hub);

                    //Fully remove users
                    FullyRemoveUsers.remove(dbContext);

                    //Clean chats
                    CleanChats.clean(dbContext);
                }
                catch (Exception)
                {
                    //Restart the timer
                    _timer?.Change(nextTime(0), new TimeSpan(1, 0, 0, 0));
                }
            }
        }
        
        /**
         * Delete the timer
         */
        public void Dispose()
        {
            _timer?.Dispose();
        }

        private static TimeSpan nextTime(int addHours)
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
