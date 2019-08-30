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
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The id of the notification socket</value>
        private static string _notificationSocketId;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// initializes the class vars
        /// </summary>
        /// <param name="conf">The configuration of the application</param>
        public static void Initialize(IConfiguration conf)
        {
            _notificationSocketId = conf["socket:inAppNotifications"];
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Send a notification to a user
        /// </summary>
        /// <param name="hub">The notification hub</param>
        /// <param name="target">The target of the notification</param>
        /// <param name="recv">The receiver of the notification</param>
        /// <param name="type">The notification type</param>
        /// <param name="context">The context of database</param>
        public static async Task send(IHubContext<NotificationHub> hub, string target, User recv, NotificationType type, ApplicationDBContext context)
        {
            string message = InAppNotificationMessages.getMessage(type, target);
            Notifications notification = new Notifications { User = recv, message = message };
            context.Add(notification);
            context.SaveChanges();

            NotificationMessage ret = new NotificationMessage { id = notification.id.ToString(), message = message };

            await hub.Clients.All.SendAsync(_notificationSocketId + recv.publicid.ToString(), ret);
        }
    }
}
