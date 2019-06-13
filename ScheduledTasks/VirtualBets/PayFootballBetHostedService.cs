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

namespace API.ScheduledTasks.VirtualBets
{
    public class PayFootballBetHostedService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory scopeFactory;
        private Timer _timer;
        private IConfiguration _configuration;

        public PayFootballBetHostedService(IServiceScopeFactory sf, IConfiguration config)
        {
            scopeFactory = sf;
            _configuration = config;
        }

       
        /**
         * Start the service
         */
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                DoWork,
                null,
                //TimeSpan.FromMinutes(2), //30 seconds from now, to wait the football database is initialized
                TimeSpan.FromHours(5), //30 minutes from now, to wait the football database is initialized (Free azure background services are so slow -.-)
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
        private async void DoWork(object state)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                    dbContext.FootballBets.Where(fb => !fb.ended && !fb.cancelled).ToList().ForEach(bet =>
                    {
                        dbContext.Entry(bet).Reference("MatchDay").Load();
                        if (bet.MatchDay.status == "FINISHED")
                        {
                            CheckWinners.checkWinner(bet, dbContext);
                            bet.ended = true;
                            dbContext.SaveChanges();
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
         * Function to get the time next day at 00:00
         * @return TimeSpan
         *      Return the TimeSpan time to the 06:00 the next day
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
                .Subtract(new DateTime(1970, 1, 1, 6, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;

            int time = ((int)(then - now)) / 1000;

            int mins = (int)(time / 60); //total min
            int hours = mins / 60; //total hours

            hours = hours % 24; //Exactly hours in one day
            mins = (mins % 60) + 1; //Exactly min in one hour

            return new TimeSpan(hours, mins, 0);
        }
    }
}
