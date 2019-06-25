using API.Data;
using System;

namespace API.Areas.Alive.Models
{
    public class ChatMessage
    {
        public ChatMessage() { }
        public ChatMessage(Data.Models.GroupChatMessage msg, ApplicationDBContext context)
        {
            context.Entry(msg).Reference("Group").Load();
            this.group = msg.Group.name;
            this.username = msg.username;
            this.publicUserId = msg.publicUserId;
            this.role = msg.role;
            this.message = msg.message;
            this.time = msg.time;
        }
        public string group { get; set; }
        public string username { get; set; }
        public string publicUserId { get; set; }
        public string role { get; set; }
        public string message { get; set; }
        public DateTime time { get; set; }
    }
}
