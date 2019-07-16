using System;

namespace API.Areas.DirectMessages.Models
{
    public class DMMessages
    {
        public DMMessages(Data.Models.DirectMessageMessages msg)
        {
            this.isAdmin = msg.isAdmin;
            this.message = msg.message;
            this.time = msg.time;
        }
        public bool isAdmin { get; set; }
        public string message { get; set; }
        public DateTime time { get; set; }
    }
}
