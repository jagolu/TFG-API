using System;

namespace API.Areas.DirectMessages.Models
{
    /// <summary>
    /// The info of a dm conversation
    /// </summary>
    public class DMTitle
    {
        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dm">The dm conversation</param>
        /// <param name="userId">The id of the caller</param>
        /// <param name="isAdmin">True if the caller is an admin, false otherwise</param>
        /// <param name="_context">The database context</param>
        public DMTitle(Data.Models.DirectMessageTitle dm, Guid userId, bool isAdmin, Data.ApplicationDBContext _context)
        {
            _context.Entry(dm).Reference("receiver").Load();
            _context.Entry(dm).Reference("Sender").Load();
            Data.Models.User recv = dm.receiver;
            Data.Models.User load = userId == recv.id ? dm.Sender : recv;

            this.id = dm.id.ToString();
            this.receiver = load.nickname;
            this.emailReceiver = isAdmin ? load.email : null;
            this.lastUpdate = dm.lastUpdate;
            this.closed = dm.closed;
            this.unreadMessages = isAdmin ? dm.unreadMessagesForAdmin : dm.unreadMessagesForUser;
            this.title = dm.title;
        }


        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //
        
        /// <value>The id of the dm conversation</value>
        public string id { get; set; }
        
        /// <value>The receiver of the conversation</value>
        public string receiver { get; set; }
        
        /// <value>The email of the receiver</value>
        public String emailReceiver { get; set; }
        
        /// <value>The time of the last update of the dm conversation</value>
        public DateTime lastUpdate { get; set; }
        
        /// <value>True if the dm conversation is closed, false otherwise</value>
        public bool closed { get; set; }
        
        /// <value>The title of the dm conversation</value>
        public string title { get; set; }
        
        /// <value>The count of the unread messages in the dm conversation by the user</value>
        public int unreadMessages { get; set; }
    }
}
