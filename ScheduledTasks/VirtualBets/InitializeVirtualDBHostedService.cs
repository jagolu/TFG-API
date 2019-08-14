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
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;
        private IConfiguration _configuration;
        private readonly IHttpClientFactory _http;

        public InitializeVirtualDBHostedService(IServiceScopeFactory sf, IConfiguration config, IHttpClientFactory http)
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
            NextScheduledTime.nextTime(0, 5);
            _timer = new Timer(
                DoWork, 
                null,
                TimeSpan.Zero, //Right now
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
            _timer?.Change(NextScheduledTime.nextTime(0, 5), new TimeSpan(1, 0, 0, 0));

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
                    //Restart the timer
                    _timer?.Change(NextScheduledTime.nextTime(0, 5), new TimeSpan(1, 0, 0, 0));
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
    }
}
