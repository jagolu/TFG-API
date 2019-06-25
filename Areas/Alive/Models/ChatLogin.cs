using System.Collections.Generic;

namespace API.Areas.Alive.Models
{
    public class ChatLogin
    {
        public string callerPublicId { get; set; }
        public List<ChatMessage> messages { get; set; }
    }
}
