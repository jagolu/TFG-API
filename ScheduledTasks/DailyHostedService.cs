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
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The scope factory to get the services</value>
        private readonly IServiceScopeFactory _scopeFactory;
        
        /// <value>A timer to se the next time of the action</value>
        private Timer _timer;
        
        /// <value>The configuration of the application</value>
        private IConfiguration _configuration;
        
        /// <value>The client factory to do the http request</value>
        private readonly IHttpClientFactory _http;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sf">The scope factory service</param>
        /// <param name="config">The configuration of the application</param>
        /// <param name="http">The http client factory</param>
        public DailyHostedService(IServiceScopeFactory sf, IConfiguration config, IHttpClientFactory http)
        {
            _scopeFactory = sf;
            _configuration = config;
            _http = http;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //        

        /**
         * Start the service
         */
        /// <summary>
        /// Starts the timing of the service
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
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

        /// <summary>
        /// Stops the timing of the service and restart it again
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            //Restart the timer
            _timer?.Change(nextTime(0), new TimeSpan(1, 0, 0, 0));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Dispose the time data
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
        }


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /**
         * Initialize the database for virtual bets
         */
        /// <summary>
        /// Update the football data in the database, pay the fb to the groups,
        /// do the weekly pay to the groups, remove the expired user session tokens, 
        /// remove completely the users who wants to remove his account and clean the chat history
        /// </summary>
        /// <param name="state">A state object</param>
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

        /// <summary>
        /// Get the timespan to the next day at the 00:00 + addHours hours
        /// </summary>
        /// <param name="addHours">The hours to add</param>
        /// <returns>The next day at the 00:00 + addHours hours</returns>
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
