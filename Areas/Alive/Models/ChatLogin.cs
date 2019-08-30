using System;
using System.Collections.Generic;

namespace API.Areas.Alive.Models
{
    /// <summary>
    /// The info that a user receives when logs in a chat
    /// </summary>
    public class ChatLogin
    {
        /// <value>The public id of the user</value>
        public string callerPublicId { get; set; }

        /// <value>The name of the chat group</value>
        public string group { get; set; }

        /// <value>The messages of the chat</value>
        public List<ChatUserMessages> userMessages { get; set; }

        /// <summary>
        /// A group of messages inside a chat
        /// </summary>
        public class ChatUserMessages
        {
            /// <value>The username of the user who sent the message</value>
            public string username { get; set; }
            
            /// <value>The public id of the user who sent the message</value>
            public string publicUserId { get; set; }

            
            /// <value>The role of the user who sent the message</value>
            public string role { get; set; }
            
            /// <value>The messages that sent the user in the same group of messages</value>
            public List<SingleUserChatMessage> messages { get; set; }
            
            /// <value>A single message sent by a user</value>
            public class SingleUserChatMessage
            {
            /// <value>The text of the message</value>
                public string message { get; set; }

                /// <value>The time when the message was sent</value>
                public DateTime time { get; set; }
            }
        }

    }
}
