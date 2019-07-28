using API.Data;
using API.ScheduledTasks.VirtualBets.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace API.ScheduledTasks.VirtualBets
{
    internal class InitializeVirtualDBHostedService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory scopeFactory;
        private Timer _timer;
        private IConfiguration _configuration;
        private readonly IHttpClientFactory _http;

        public InitializeVirtualDBHostedService(IServiceScopeFactory sf, IConfiguration config, IHttpClientFactory http)
        {
            scopeFactory = sf;
            _configuration = config;
            _http = http;
        }


        /**
         * Start the service
         */
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                DoWork, 
                null,
                //TimeSpan.Zero, //Right now
                CalculateInitalNextTime(), //Right now
                CalculateInitalNextTime() //The next first August or the next month
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
        private async void DoWork(object state)
        {
            //TODO initialize the database
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                    InitializerVirtualDB initializer = new InitializerVirtualDB(dbContext, _configuration, _http);
                    
                    await initializer.InitializeAsync("PL");
                    await initializer.InitializeAsync("PD");
                    await initializer.InitializeAsync("BL1");
                    await initializer.InitializeAsync("SA");
                    await initializer.InitializeAsync("FL1");
                }
                catch (Exception)
                {
                    _timer?.Change(TimeSpan.FromDays(1), TimeSpan.FromDays(2));
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


        /**
         * Function to get the time to the next day at 00:05
         * @return TimeSpan
         *      Return the TimeSpan time to the next day at 00:05
         */
        private TimeSpan CalculateInitalNextTime()
        {
            DateTime nowDay = DateTime.Now;
            nowDay = new DateTime(nowDay.Year, nowDay.Month, nowDay.Day); //Actual day at 00:00

            double now = DateTime.Now
                .ToUniversalTime()
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;

            double then = nowDay.AddDays(1)
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
