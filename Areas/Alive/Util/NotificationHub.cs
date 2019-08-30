using API.Data;
using API.Data.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.Alive.Util
{
    public class NotificationHub : Hub
    {
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>Scope factory to get the database context</value>
        private readonly IServiceScopeFactory scopeFactory;

        /// <summary>The id of the notification socket</summary>
        private readonly string _notificationSocketId;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sf">Scope factory</param>
        /// <param name="configuration">The configuration of the application</param>
        public NotificationHub(IServiceScopeFactory sf, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            scopeFactory = sf;
            _notificationSocketId = configuration["socket:inAppNotifications"];
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Subscibes a user to his notification hub
        /// </summary>
        /// <param name="publicUserId">The public id of the user who will receive the notification</param>
        public async Task BroadcastNotificationsData(string publicUserId)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    ApplicationDBContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                    List<User> us = dbContext.User.Where(u => u.publicid == publicUserId).ToList();
                    if (us.Count() != 1) return;

                    await Clients.All.SendAsync(_notificationSocketId + publicUserId, "CONNECTED");
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
