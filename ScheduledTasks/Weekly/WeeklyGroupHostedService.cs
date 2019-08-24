using API.Areas.Alive.Util;
using API.Data;
using API.ScheduledTasks.VirtualBets.Util;
using API.ScheduledTasks.Weekly.Util;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace API.ScheduledTasks.Weekly
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
                NextScheduledTime.nextTime(5), //Initial time
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
            _timer?.Change(NextScheduledTime.nextTime(5), new TimeSpan(1, 0, 0, 0));

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
                    CleanChats.clean(dbContext);
                }
            }
            catch (Exception)
            {
                //Restart the timer
                _timer?.Change(NextScheduledTime.nextTime(5), new TimeSpan(1, 0, 0, 0));
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
