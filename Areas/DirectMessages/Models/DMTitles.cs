using System;

namespace API.Areas.DirectMessages.Models
{
    public class DMTitles
    {
        public DMTitles(Data.Models.DirectMessageTitle dm, bool isAdmin, Data.ApplicationDBContext _context)
        {
            _context.Entry(dm).Reference("Receiver").Load();
            Data.Models.User recv = dm.Receiver;

            this.id = dm.id.ToString();
            this.receiver = recv.nickname;
            this.emailReceiver = isAdmin ? recv.email : null;
            this.openDate = dm.openDate;
            this.closed = dm.closed;
            this.unreadMessages = isAdmin ? dm.unreadMessagesForAdmin : dm.unreadMessagesForUser;
            this.title = dm.title;
        }

        public string id { get; set; }
        public string receiver { get; set; }
        public String emailReceiver { get; set; }
        public DateTime openDate { get; set; }
        public bool closed { get; set; }
        public string title { get; set; }
        public int unreadMessages { get; set; }
    }
}
