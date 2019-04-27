using API.Data;
using API.Data.Models;
using API.ScheduledTasks.InitializeVirtualDB.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace API.ScheduledTasks.InitializeVirtualDBHostedService
{
    internal class InitializeVirtualDBHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory scopeFactory;
        private Timer _timer;
        private IConfiguration _configuration;
        private readonly IHttpClientFactory _http;

        public InitializeVirtualDBHostedService(ILogger<InitializeVirtualDBHostedService> logger, IServiceScopeFactory sf, IConfiguration config, IHttpClientFactory http)
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
            _logger.BeginScope("Initialize virtual DB");
            _logger.LogInformation("InitializeVirtualDB Service is starting"+ CalculateInitalNextTime());
            _timer = new Timer(
                DoWork, 
                null,
                TimeSpan.Zero, //Right now
                CalculateInitalNextTime() //The next first August or the next month
            );

            return Task.CompletedTask;
        }


        /**
         * If the application fails, the task and timer will cancel
         */
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("InitializeVirtualDB Service is stopping.");

            //Stop the timer
            _timer.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /**
         * Initialize the database for virtual bets
         */
        private async void DoWork(object state)
        {
            _logger.LogInformation("InitializeVirtualDB Service is working.");
            //TODO initialize the database
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                bool correct = true;

                //VirtualDB is initialized
                if (dbContext.Competitions.Count() == 0)
                {
                    InitializerVirtualDB initializer = new InitializerVirtualDB(dbContext, _configuration, _http, _logger);
                    correct = await initializer.InitializeAsync();
                }
                else
                {
                    int actualMonth = DateTime.Now.Month;
                    int actualDay = DateTime.Now.Day;

                    if (actualMonth == 8 && actualDay == 1)
                    {
                        //Clean the DB and reinitialize
                        CleanVirtualDB(dbContext);
                        InitializerVirtualDB initializer = new InitializerVirtualDB(dbContext, _configuration, _http, _logger);
                        correct = await initializer.InitializeAsync();
                    }
                }

                //If the DB wasn't initialized correctly try again next day
                if (!correct) _timer?.Change(new TimeSpan(1, 0, 0, 0), new TimeSpan(1, 0, 0, 0));
                else _timer?.Change(CalculateInitalNextTime(), CalculateInitalNextTime()); //Reinitialize the first August next year
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
         * Function to get the time to first August or the next month
         * @return TimeSpan
         *      Return the TimeSpan time to first August if there is less than 31
         *      days till that day. Return 31 TimeSpan days otherwise.
         */
        private TimeSpan CalculateInitalNextTime()
        {
            int moreYears = 0;

            double now =  DateTime.Now
                .ToUniversalTime()
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;

            if (DateTime.Now.Month > 10) moreYears++;

            double then = new DateTime(DateTime.Now.Year + moreYears, 8, 1)
                .ToUniversalTime()
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;

            long time = (long)(then - now);

            if (time > 2678400000) //2678400000 -> 31 days //There is more than 31 days till first August
            {
                return new TimeSpan(31, 0, 0, 0); //31 days
            }

            //convert milisecond in days, hours, seconds  //Less than 31 days till first August
            time = time / 1000; //time in seconds

            int mins = (int)(time / 60); //total min
            int hours = mins / 60; //total hours
            int days = hours / 24; //exactly days in one year

            hours = hours % 24; //Exactly hours in one day
            mins = (mins % 60)+1; //Exactly min in one hour

            return new TimeSpan(days, hours, mins, 0);
        }


        /**
         * Function to delete al entries of the (Teams, MatchDays & Competitions) tables
         */
        private void CleanVirtualDB(ApplicationDBContext _context)
        {
            _context.MatchDays.ToList().ForEach(matchD =>
             {
                 _context.Remove(matchD);
             });

            _context.Teams.ToList().ForEach(team =>
            {
                _context.Remove(team);
            });

            _context.Competitions.ToList().ForEach(competition =>
            {
                _context.Remove(competition);
            });

            _context.SaveChanges();
        }
    }
}
