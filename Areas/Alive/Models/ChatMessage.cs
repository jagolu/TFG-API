using System;

namespace API.Areas.Alive.Models
{
    public class ChatMessage
    {
        public string group { get; set; }
        public string username { get; set; }
        public string publicUserId { get; set; }
        public string role { get; set; }
        public string message { get; set; }
        public DateTime time { get; set; }
    }
}
