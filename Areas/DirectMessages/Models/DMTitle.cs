﻿using System;

namespace API.Areas.DirectMessages.Models
{
    public class DMTitle
    {
        public DMTitle(Data.Models.DirectMessageTitle dm, Guid userId, bool isAdmin, Data.ApplicationDBContext _context)
        {
            _context.Entry(dm).Reference("Receiver").Load();
            _context.Entry(dm).Reference("Sender").Load();
            Data.Models.User recv = dm.Receiver;
            Data.Models.User load = userId == recv.id ? dm.Sender : recv;

            this.id = dm.id.ToString();
            this.receiver = load.nickname;
            this.emailReceiver = isAdmin ? load.email : null;
            this.lastUpdate = dm.lastUpdate;
            this.closed = dm.closed;
            this.unreadMessages = isAdmin ? dm.unreadMessagesForAdmin : dm.unreadMessagesForUser;
            this.title = dm.title;
        }

        public string id { get; set; }
        public string receiver { get; set; }
        public String emailReceiver { get; set; }
        public DateTime lastUpdate { get; set; }
        public bool closed { get; set; }
        public string title { get; set; }
        public int unreadMessages { get; set; }
    }
}