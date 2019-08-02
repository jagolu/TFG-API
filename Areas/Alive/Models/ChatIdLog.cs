using System.Collections.Generic;

namespace API.Areas.Alive.Models
{
    public class ChatIdLog
    {
        public string groupName { get; set; }
        public List<CoupleUserConnectionId> users { get; set; }
    }
}
