using System.Collections.Generic;

namespace API.Areas.Alive.Models
{
    public class LoginNotifications
    {
        public string publicUserid { get; set; }
        public List<NotificationMessage> messages { get; set; }
    }
}
