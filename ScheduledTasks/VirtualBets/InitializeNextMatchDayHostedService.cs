using API.Data;
using API.ScheduledTasks.VirtualBets.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace API.ScheduledTasks.InitializeNextMatchDay
{
    public class InitializeNextMatchDayHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory scopeFactory;
        private Timer _timer;
        private IConfiguration _configuration;
        private readonly IHttpClientFactory _http;

        public InitializeNextMatchDayHostedService(ILogger<InitializeNextMatchDayHostedService> logger, IServiceScopeFactory sf, IConfiguration config, IHttpClientFactory http)
        {
            _logger = logger;
            scopeFactory = sf;
            _configuration = config;
            _http = http;
        }


        /**
         * Start the service
         */
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.BeginScope("Initialize next day match");

            _timer = new Timer(
                DoWork,
                null,
                //TimeSpan.Zero, //30 seconds from now, to wait the football database is initialized
                TimeSpan.FromSeconds(120), //30 seconds from now, to wait the football database is initialized
                CalculateInitalNextTime() //tomorrow
            );

            return Task.CompletedTask;
        }


        /**
         * If the application fails, the task and timer will cancel
         */
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("InitializeNextMatchDay Service is stopping.");

            //Stop the timer
            _timer.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }


        /**
         * Initialize the database for virtual bets
         */
        private async void DoWork(object state)
        {
            _logger.LogInformation("InitializeNextMatchDay Service is working.");
            //TODO initialize the database
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                InitializerNextMatchDay initializer = new InitializerNextMatchDay(dbContext, _configuration, _http);

                if (dbContext.Competitions.Count()!=0) //The competitions are intialized
                {
                    int currentMatchDayPL = dbContext.Competitions.Where(c => c.name == "Premier League").First().actualMatchDay;
                    int currentMatchDayPD = dbContext.Competitions.Where(c => c.name == "Primera Division").First().actualMatchDay;

                    if (currentMatchDayPD+1 < 39) await initializer.InitializeAsync("PD", currentMatchDayPD+1);
                    if (currentMatchDayPL+1 < 39) await initializer.InitializeAsync("PL", currentMatchDayPL+1);
                }

                //Set cron next day
                _timer?.Change(CalculateInitalNextTime(), CalculateInitalNextTime());
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
         * Function to get the time next day at 00:00
         * @return TimeSpan
         *      Return the TimeSpan time to the 00:00 the next day
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
            //int days = hours / 24; //exactly days in one year

            hours = hours % 24; //Exactly hours in one day
            mins = (mins % 60) + 1; //Exactly min in one hour

            return new TimeSpan(hours, mins, 0);
        }
    }
}
