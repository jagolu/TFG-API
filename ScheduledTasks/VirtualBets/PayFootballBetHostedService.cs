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
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;

        public PayFootballBetHostedService(IServiceScopeFactory sf)
        {
            _scopeFactory = sf;
        }

       
        /**
         * Start the service
         */
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
               DoWork,
               null,
               NextScheduledTime.nextTime(3, 5), //Initial time
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
            _timer?.Change(NextScheduledTime.nextTime(3), new TimeSpan(1, 0, 0, 0));

            return Task.CompletedTask;
        }

        /**
         * Initialize the database for virtual bets
         */
        private void DoWork(object state)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
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
                        }
                    });
                }
            }
            catch (Exception)
            {
                //Restart the timer
                _timer?.Change(NextScheduledTime.nextTime(3), new TimeSpan(1, 0, 0, 0));
            }
        }

        /**
         * Delete the timer
         */
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
