using API.Areas.Alive.Models;
using API.Data;
using API.Data.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace API.Areas.Alive.Util
{
    public static class SendNotification
    {
        private static string __notificationSocketId;

        public static void Initialize(IConfiguration conf)
        {
            __notificationSocketId = conf["socket:inAppNotifications"];
        }


        public static async Task send(IHubContext<NotificationHub> hub, string target, User recv, NotificationType type, ApplicationDBContext context)
        {
            string message = InAppNotificationMessages.getMessage(type, target);
            Notifications notification = new Notifications { User = recv, message = message };
            context.Add(notification);
            context.SaveChanges();

            NotificationMessage ret = new NotificationMessage { id = notification.id.ToString(), message = message };

            await hub.Clients.All.SendAsync(__notificationSocketId + recv.publicId.ToString(), ret);
        }
    }
}
