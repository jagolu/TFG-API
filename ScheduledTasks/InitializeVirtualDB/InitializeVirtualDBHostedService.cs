using API.Data;
using API.Data.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.ScheduledTasks.InitializeVirtualDBHostedService
{
    internal class InitializeVirtualDBHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory scopeFactory;
        private Timer _timer;

        public InitializeVirtualDBHostedService(ILogger<InitializeVirtualDBHostedService> logger, IServiceScopeFactory sf)
        {
            _logger = logger;
            scopeFactory = sf;
        }

        /**
         * Start the service
         */
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.BeginScope("Init");
            _logger.Log(LogLevel.Trace, "asdfasdf");
            _logger.LogInformation("InitializeVirtualDB Service is starting");

            _timer = new Timer(DoWork, null, 0, CalculateInitalNextTime());

            return Task.CompletedTask;
        }

        /**
         * If the application fails, the task and timer
         * will cancel
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
        private void DoWork(object state)
        {
            //Set the timer to the next first of August and a year after it
            _timer?.Change(CalculateInitalNextTime(), 5000);
            _logger.LogInformation("InitializeVirtualDB Service is working.");
            //TODO initialize the database
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
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
         * Calculate the time in miliseconds till the next
         * first of August.
         */
        private int CalculateInitalNextTime()
        {
            int now = (int) DateTime.Now
                .ToUniversalTime()
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;

            int moreYears = 0;

            if (DateTime.Now.Month > 10)
            {
                moreYears++;
            }

            int then =(int) new DateTime(DateTime.Now.Year + moreYears, 8, 1)
                .ToUniversalTime()
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;

            return then - now;
        }
    }
}
