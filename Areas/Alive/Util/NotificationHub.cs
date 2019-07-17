using API.Areas.Alive.Models;
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
        private readonly string __key;

        public NotificationHub(IServiceScopeFactory sf, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            scopeFactory = sf;
            notificationSocketId = configuration["socket:inAppNotifications"];
            __key = configuration["socket:inAppNotificationsKey"];
        }

        public async Task BroadcastNotificationsData(string id, string publicUserId)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    ApplicationDBContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                    List<User> us = dbContext.User.Where(u => u.publicId == publicUserId).ToList();
                    if (us.Count() != 1) return;

                    User user = us.First();
                    List<Notifications> userNotifications = user.notifications.Where(n => n.id.ToString() == id).ToList();

                    if (userNotifications.Count() != 1) return;
                    Notifications notfication = userNotifications.First();

                    dbContext.Remove(notfication);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        public async Task sendNotificationFromServer(NotificationType type, string target, string userid, string key)
        {
            if (__key != key) return;

            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    ApplicationDBContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                    List<User> us = dbContext.User.Where(u => u.id.ToString() == userid).ToList();
                    if (us.Count() != 1) return;

                    User user = us.First();
                    string message = InAppNotificationMessages.getMessage(type, target);
                    Notifications n = new Notifications { User = user, message = message };

                    NotificationMessage retMessage = new NotificationMessage { id = userid, message = message };

                    await Clients.All.SendAsync(notificationSocketId + userid, message); ;
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
