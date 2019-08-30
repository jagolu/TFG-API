using System.Collections.Generic;

namespace API.Areas.Alive.Models
{
    /// <summary>
    /// The response when a user logs into the notifications
    /// </summary>
    public class LoginNotifications
    {
        /// <value>The public id of the user who has logged</value>
        public string publicUserid { get; set; }
        
        /// <value>The messages in the chat</value>
        public List<NotificationMessage> messages { get; set; }
    }
}
