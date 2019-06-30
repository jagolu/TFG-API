using System;
using System.Collections.Generic;

namespace API.Areas.Alive.Models
{
    public class ChatLogin
    {
        public string callerPublicId { get; set; }
        public string group { get; set; }
        public List<ChatUserMesssages> userMessages { get; set; }

        public class ChatUserMesssages
        {
            public string username { get; set; }
            public string publicUserId { get; set; }
            public string role { get; set; }
            public List<SingleUserChatMessage> messages { get; set; }

            public class SingleUserChatMessage
            {
                public string message { get; set; }
                public DateTime time { get; set; }
            }
        }

    }
}
