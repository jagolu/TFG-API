using System;

namespace API.Areas.DirectMessages.Models
{
    /// <summary>
    /// A message in a dm conversation
    /// </summary>
    public class DMMessage
    {
        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="msg">A message in a dm conversation</param>
        public DMMessage(Data.Models.DirectMessageMessages msg)
        {
            this.message = msg.message;
            this.time = msg.time;
        }


        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //
        
        /// <value>The text of the message</value>
        public string message { get; set; }
        
        /// <value>The time when the message was sent</value>
        public DateTime time { get; set; }
    }
}
