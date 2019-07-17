using API.Data.Models;
using System.Collections.Generic;

namespace API.Areas.DirectMessages.Models
{
    public class DMMessageCluster
    {
        public DMMessageCluster(List<DirectMessageMessages> msgs, bool isAdmin)
        {
            this.isAdmin = isAdmin;
            this.messages = new List<DMMessage>();
            msgs.ForEach(m => this.messages.Add(new DMMessage(m)));
        }
        public bool isAdmin { get; set; }
        public List<DMMessage> messages { get; set; }
    }
}
