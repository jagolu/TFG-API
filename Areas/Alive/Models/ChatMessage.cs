using System;

namespace API.Areas.Alive.Models
{
    /// <summary>
    /// A message sent to a group chat
    /// </summary>
    public class ChatMessage
    {
        /// <value>The group chat</value>
        public string group { get; set; }

        /// <value>The user who sent the message</value>
        public string username { get; set; }

        /// <value>The public id of the user</value>
        public string publicUserId { get; set; }

        /// <value>The role of the user</value>
        public string role { get; set; }
        
        /// <value>The text of the message</value>
        public string message { get; set; }
        
        /// <value>The time when the message was sent</value>
        public DateTime time { get; set; }
    }
}
