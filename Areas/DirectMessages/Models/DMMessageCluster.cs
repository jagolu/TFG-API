using API.Data.Models;
using System.Collections.Generic;

namespace API.Areas.DirectMessages.Models
{
    /// <summary>
    /// A cluster of the same user messages in a row in a dm conversation
    /// </summary>
    public class DMMessageCluster
    {
        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="msgs">The messages in a dm conversation</param>
        /// <param name="isAdmin">True if the caller is an admin, false otherwise</param>
        public DMMessageCluster(List<DirectMessageMessages> msgs, bool isAdmin)
        {
            this.isAdmin = isAdmin;
            this.messages = new List<DMMessage>();
            msgs.ForEach(m => this.messages.Add(new DMMessage(m)));
        }


        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //        
        
        /// <value>True if the sender is an admin, false otherwise</value>
        public bool isAdmin { get; set; }
        
        /// <value>The list of messsages by the same user in the a period of time</value>
        public List<DMMessage> messages { get; set; }
    }
}
