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
        private readonly IServiceScopeFactory scopeFactory;
        private readonly string notificationSocketId;

        public NotificationHub(IServiceScopeFactory sf, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            scopeFactory = sf;
            notificationSocketId = configuration["socket:inAppNotifications"];
        }

        public async Task BroadcastNotificationsData(string publicUserId)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    ApplicationDBContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                    List<User> us = dbContext.User.Where(u => u.publicid == publicUserId).ToList();
                    if (us.Count() != 1) return;

                    await Clients.All.SendAsync(notificationSocketId + publicUserId, "CONNECTED");
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
