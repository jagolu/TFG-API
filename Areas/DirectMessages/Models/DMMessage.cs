using System;

namespace API.Areas.DirectMessages.Models
{
    public class DMMessage
    {
        public DMMessage(Data.Models.DirectMessageMessages msg)
        {
            this.message = msg.message;
            this.time = msg.time;
        }
        public string message { get; set; }
        public DateTime time { get; set; }
    }
}
